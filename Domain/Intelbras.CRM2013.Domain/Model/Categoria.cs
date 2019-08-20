using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    [LogicalEntity("itbc_categoria")]
    public class Categoria : DomainBase
    {
        #region Construtores
        
            private RepositoryService RepositoryService { get; set; }

        public Categoria() { }

        public Categoria(string organization, bool isOffline)
                : base(organization, isOffline)
            {
                RepositoryService = new RepositoryService(organization, isOffline);
            }

            public Categoria(string organization, bool isOffline, object provider)
                : base(organization, isOffline, provider)
            {
                RepositoryService = new RepositoryService(organization, isOffline, provider);
            }

        #endregion

        #region Atributos
            [LogicalAttribute("itbc_categoriaid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("itbc_name")]
            public String Nome {get; set;}

            [LogicalAttribute("itbc_codigo_categoria")]
            public String CodigoCategoria {get; set;}

        #endregion
    }
}
