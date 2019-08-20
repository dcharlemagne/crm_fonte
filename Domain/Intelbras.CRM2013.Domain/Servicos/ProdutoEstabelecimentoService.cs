using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ProdutoEstabelecimentoService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ProdutoEstabelecimentoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public ProdutoEstabelecimentoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        public bool VerificarExistenciaProduto(ProdutoEstabelecimento prodEstab)
        {
            List<ProdutoEstabelecimento> lstProdEstab = new List<ProdutoEstabelecimento>();
            lstProdEstab = RepositoryService.ProdutoEstabelecimento.ListarPorProduto(prodEstab.Produto.Id);

            if (lstProdEstab.Count() > 0)
                return true;

            return false;
        }


        public ProdutoEstabelecimento ObterPor(Guid prodEstabId)
        {
            return RepositoryService.ProdutoEstabelecimento.ObterPor(prodEstabId);
        }

        public ProdutoEstabelecimento ObterPorProduto(Guid produtoId)
        {
            return RepositoryService.ProdutoEstabelecimento.ObterPorProduto(produtoId);
        }

        public List<ProdutoEstabelecimento> ListarPorEstabelecimento(Guid prodEstab)
        {

            List<ProdutoEstabelecimento> lstProdEstab = RepositoryService.ProdutoEstabelecimento.ListarPorEstabelecimento(prodEstab);

            if (lstProdEstab.Count() > 0)
                return lstProdEstab;

            return null;
        }

        public List<ProdutoEstabelecimento> ListarTodos()
        {
            List<ProdutoEstabelecimento> lstProdEstab = RepositoryService.ProdutoEstabelecimento.ListarTodos();

            if (lstProdEstab.Count() > 0)
                return lstProdEstab;

            return null;
        }

        public ProdutoEstabelecimento Persistir(Model.ProdutoEstabelecimento objProdEstab)
        {
            ProdutoEstabelecimento TmpProdEsta = null;
            if (objProdEstab.ID.HasValue)
            {
                TmpProdEsta = RepositoryService.ProdutoEstabelecimento.ObterPor(objProdEstab.ID.Value);

                if (TmpProdEsta != null)
                {
                    objProdEstab.ID = TmpProdEsta.ID;
                    RepositoryService.ProdutoEstabelecimento.Update(objProdEstab);
                    //Altera Status - Se necessário
                    if (!TmpProdEsta.Status.Equals(objProdEstab.Status) && objProdEstab.Status != null)
                        this.MudarStatus(TmpProdEsta.ID.Value, objProdEstab.Status.Value);
                    return TmpProdEsta;
                }
                else
                    return null;
            }
            else
            {
                objProdEstab.ID = RepositoryService.ProdutoEstabelecimento.Create(objProdEstab);
                return objProdEstab;
            }
        }

        public void Excluir(Guid prodEstabId)
        {
            RepositoryService.ProdutoEstabelecimento.Delete(prodEstabId);
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.ProdutoEstabelecimento.AlterarStatus(id, status);
        }

        public string IntegracaoBarramento(ProdutoEstabelecimento objProdEstabelecimento)
        {
            Domain.Integracao.MSG0139 msgProdEstab = new Domain.Integracao.MSG0139(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            return msgProdEstab.Enviar(objProdEstabelecimento);
        }

        public string IntegracaoBarramentoDelete(ProdutoEstabelecimento objProdEstabelecimento)
        {
            Domain.Integracao.MSG0140 msgProdEstabDel = new Domain.Integracao.MSG0140(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            return msgProdEstabDel.Enviar(objProdEstabelecimento);
        }


    }
}