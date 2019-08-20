using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_itbc_politicacomercial_account")]
    public class PoliticaComercialXConta : DomainBase
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public PoliticaComercialXConta(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public PoliticaComercialXConta(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_itbc_politicacomercial_accountid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_politicacomercialid")]
        public Guid? PoliticaComercialId { get; set; }

        [LogicalAttribute("accountid")]
        public Guid? ContaId { get; set; }

        #endregion
    }
}