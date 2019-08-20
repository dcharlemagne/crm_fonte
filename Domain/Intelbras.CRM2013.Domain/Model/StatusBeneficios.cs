using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_statusbeneficios")]
    public class StatusBeneficios : DomainBase
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public StatusBeneficios() { }

        public StatusBeneficios(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public StatusBeneficios(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_statusbeneficiosid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }

        #endregion
    }
}
