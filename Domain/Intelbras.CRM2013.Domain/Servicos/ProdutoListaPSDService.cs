using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ProdutoListaPSDService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ProdutoListaPSDService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
        }


        public ProdutoListaPSDService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        public bool ValidarExistencia(ref Model.ProdutoListaPSDPPPSCF produto)
        {
            
            if (produto.Produto != null)
            {
                var lista = new List<Model.ProdutoListaPSDPPPSCF>();
                Model.ProdutoListaPSDPPPSCF prodpost = RepositoryService.ProdutoListaPSD.Retrieve(produto.ID.Value);

                if (prodpost.PSD.Id != produto.PSD.Id || prodpost.Produto.Id != produto.Produto.Id)
                    lista = RepositoryService.ProdutoListaPSD.ListarPor(produto.PSD.Id, produto.Produto.Id);

                return lista.Count > 0 ? true : false;
            }
            else
                return false;
        }

        public bool ValidarExistenciaPreCreate(ref Model.ProdutoListaPSDPPPSCF produto)
        {
            var lista = new List<Model.ProdutoListaPSDPPPSCF>();
            lista = RepositoryService.ProdutoListaPSD.ListarPor(produto.PSD.Id, produto.Produto.Id);

            return lista.Count > 0 ? true : false;
        }
    }
}
