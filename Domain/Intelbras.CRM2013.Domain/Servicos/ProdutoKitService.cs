using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ProdutoKitService
    {
        private RepositoryService RepositoryService { get; set; }

        #region Construtores
        
        public ProdutoKitService(string organizacao, bool isOffline) : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public ProdutoKitService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public ProdutoKitService(RepositoryService repository)
        {
            RepositoryService = repository;
        }

        #endregion

        public void ExcluirCriarPorProdutoPai(Guid produtoPaiId, List<Model.ProdutoKit> listaProdutoKitNovos)
        {
            List<Model.ProdutoKit> listaProdutoKitAtual = RepositoryService.ProdutoKit.ListarPorProdutoPai(produtoPaiId);

            foreach (var item in listaProdutoKitAtual)
            {
                RepositoryService.ProdutoKit.Delete(item.ID.Value);
            }

            foreach (var item in listaProdutoKitNovos)
            {
                RepositoryService.ProdutoKit.Create(item);
            }
        }
    }
}
