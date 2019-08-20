using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_itbc_segmentocomercial_account")]
    public class SegmentoComercialConta: IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public SegmentoComercialConta() { }

        public SegmentoComercialConta(string organization, bool isOffline)
                : base(organization, isOffline)
            {
                RepositoryService = new RepositoryService(organization, isOffline);
            }

            public SegmentoComercialConta(string organization, bool isOffline, object provider)
                : base(organization, isOffline, provider)
            {
                RepositoryService = new RepositoryService(organization, isOffline, provider);
            }

        #endregion

        #region Atributos
            [LogicalAttribute("itbc_itbc_segmentocomercial_accountid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("itbc_segmentocomercialid")]
            public Guid? SegmentoComercial { get; set; }

            [LogicalAttribute("accountid")]
            public Guid? Conta { get; set; }
        
        #endregion
    }
}
