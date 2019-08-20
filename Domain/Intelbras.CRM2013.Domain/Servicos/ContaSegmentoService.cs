using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ContaSegmentoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ContaSegmentoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public ContaSegmentoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public ContaSegmentoService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        #region Métodos

        public ContaSegmento Persistir(ContaSegmento contaSegmento)
        {
            return null;
        }

        public ContaSegmento Busca(Guid id)
        {
            List<ContaSegmento> lstContaSegmento = RepositoryService.ContaSegmento.ListarPor(id);
            if (lstContaSegmento.Count > 0)
                return lstContaSegmento.FirstOrDefault();
            return null;
        }

        public void AtualizaSegmentos(ContaSegmento contaSegmento)
        {
            CanalVerdeService canalverdeService = new CanalVerdeService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            FamiliaProdutoService familiaProdutoService = new FamiliaProdutoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            ContaService contaService = new ContaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            var listaVerde = canalverdeService.listarPorContaTodos(contaSegmento.Conta.Id);
            var listaFamiliaProduto = familiaProdutoService.ListarPorSegmento(contaSegmento.Segmento.Id, true, null, null);
            var conta = contaService.BuscaConta(contaSegmento.Conta.Id);
            
            //MaisVerde = Sim
            if (contaSegmento.MaisVerde)
            {
                //percorre a lista de produtos
                foreach (var item in listaFamiliaProduto)
                {
                    var verde = listaVerde.Where(x => x.FamiliaProduto.Id.Equals(item.ID)).FirstOrDefault();
                    if (verde != null)
                    {
                        //se esta inativo, ativa e salva
                        if (verde.Status.Equals(1))
                        {
                            verde.Status = 0;
                            canalverdeService.Persistir(verde);
                        }
                    }
                    else
                    {
                        //cria um novo registro
                        CanalVerde canalVerde = new CanalVerde();
                        canalVerde.Canal = contaSegmento.Conta;
                        canalVerde.Segmento = item.Segmento;
                        canalVerde.FamiliaProduto = new SDKore.DomainModel.Lookup() { Id = item.Id };
                        canalVerde.Nome = conta.CodigoMatriz + " | " + item.Segmento.Name + " | " + item.Nome;
                        canalVerde.Status = 0;

                        canalverdeService.Persistir(canalVerde);
                    }
                }
            }
            else
            {
                //percorre a lista de produtos
                foreach (var item in listaFamiliaProduto)
                {
                    var verde = listaVerde.Where(x => x.FamiliaProduto.Id.Equals(item.ID)).FirstOrDefault();
                    if (verde != null)
                    {
                        //se esta ativo, inativa e salva
                        if (verde.Status.Equals(0))
                        {
                            verde.Status = 1;
                            canalverdeService.Persistir(verde);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
