using System;
using SDKore.Crm.Util;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_categoriadob2b")]
    public class CategoriaB2B : DomainBase
    {
        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public CategoriaB2B()
        {
        }

        public CategoriaB2B(string organization, bool isOffline)
                : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public CategoriaB2B(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
            [LogicalAttribute("itbc_categoriadob2bid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("itbc_name")]
            public String Nome {get; set;}

            [LogicalAttribute("itbc_codigocategoriab2b")]
            public int? CodigoCategoriaB2B {get; set;}
        #endregion
    }
}
