using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    public class ProdutoLista : DomainBase
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ProdutoLista() { }

        public ProdutoLista(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ProdutoLista(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos

        public Guid ProdutoId { get; set; }

        public int TipoPortifolio { get; set; }
        
        #endregion
    }
}
