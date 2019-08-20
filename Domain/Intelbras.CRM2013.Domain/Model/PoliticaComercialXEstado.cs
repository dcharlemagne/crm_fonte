using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_itbc_politicacomercial_itbc_estado")]
    public class PoliticaComercialXEstado : DomainBase
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public PoliticaComercialXEstado(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public PoliticaComercialXEstado(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_itbc_politicacomercial_itbc_estadoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_politicacomercialid")]
        public Guid? PoliticaComercialId { get; set; }

        [LogicalAttribute("itbc_estadoid")]
        public Guid? EstadoId { get; set; }

        #endregion
    }
}
