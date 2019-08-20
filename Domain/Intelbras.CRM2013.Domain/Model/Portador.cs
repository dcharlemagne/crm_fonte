using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_portador")]
    public class Portador : IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Portador() { }

        public Portador(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public Portador(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_portadorid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_codigo_portador")]
        public Int32? CodigoPortador { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("statecode")]
        public int? Status { get; set; }
        #endregion
    }
}
