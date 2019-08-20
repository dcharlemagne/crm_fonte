using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ProdutoProjetoService
    {

        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public ProdutoProjetoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public ProdutoProjetoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public ProdutoProjetoService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        #region Métodos

        public List<ProdutoProjeto> Persistir(List<ProdutoProjeto> lstProdutosProjeto)
        {
            List<ProdutoProjeto> produtosTemp = new List<ProdutoProjeto>();
            foreach (var produto in lstProdutosProjeto)
            {
                var retornoItem = Persistir(produto);
                if (retornoItem == null)
                {
                    throw new ArgumentException("(CRM) Erro de Persistência no Produto do Projeto!");
                }
                produtosTemp.Add(retornoItem);
            }

            return produtosTemp;
        }

        public void AtualizarListagemProdutos(Guid clientePotencialId, List<ProdutoProjeto> lstProdutosProjetoAtuais)
        {
            var listaOld = ListarPorClientePotencial(clientePotencialId);

            foreach (var produtoProjetoOld in listaOld)
            {
                if (! lstProdutosProjetoAtuais.Exists(x => x.ID.Equals(produtoProjetoOld.ID)) || lstProdutosProjetoAtuais.Count == 0)
                {
                     RepositoryService.ProdutoProjeto.Delete(produtoProjetoOld.ID.Value);
                }
            }
        }

        public ProdutoProjeto ObterPor(Guid produtoProjetoId, params string[] columns)
        {
            return RepositoryService.ProdutoProjeto.Retrieve(produtoProjetoId, columns);
        }
    
        public List<ProdutoProjeto> ListarPorClientePotencial(Guid clientePotencialId)
        {
            return RepositoryService.ProdutoProjeto.ListarPorClientePotencial(clientePotencialId);
        }

        public ProdutoProjeto Persistir(ProdutoProjeto objProdutodoProjeto)
        {
            ProdutoProjeto TmpProdProjeto = null;
            if (objProdutodoProjeto.ID.HasValue)
                TmpProdProjeto = ObterPor(objProdutodoProjeto.ID.Value);

            if (TmpProdProjeto != null)
            {
                RepositoryService.ProdutoProjeto.Update(objProdutodoProjeto);
                return TmpProdProjeto;
            }
            else
                objProdutodoProjeto.ID = RepositoryService.ProdutoProjeto.Create(objProdutodoProjeto);
            return objProdutodoProjeto;
        }

        #endregion
    }
}
