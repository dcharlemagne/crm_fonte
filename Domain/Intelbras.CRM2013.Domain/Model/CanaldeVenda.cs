using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_canaldevenda")]
    public class CanaldeVenda : IntegracaoBase
    {
        #region Construtores

            private RepositoryService RepositoryService { get; set; }

        public CanaldeVenda() { }

        public CanaldeVenda(string organization, bool isOffline)
                : base(organization, isOffline)
            {
                RepositoryService = new RepositoryService(organization, isOffline);
            }

            public CanaldeVenda(string organization, bool isOffline, object provider)
                : base(organization, isOffline, provider)
            {
                RepositoryService = new RepositoryService(organization, isOffline, provider);
            }

        #endregion

        #region Atributos

            [LogicalAttribute("itbc_canaldevendaid")]
            public Guid? ID {get; set;}

            [LogicalAttribute("itbc_name")]
            public String Nome {get; set;}

            [LogicalAttribute("itbc_codigo_venda")]
            public int? CodigoVenda {get; set;}

        #endregion
    }
}
