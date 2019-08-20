using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_referenciasdocanal")]
    public class ReferenciasCanal : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ReferenciasCanal() { }

        public ReferenciasCanal(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        public ReferenciasCanal(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_referenciasdocanalid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_accountid")]
        public Lookup Canal { get; set; }

        [LogicalAttribute("itbc_contato")]
        public String Contato { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_telefone")]
        public String Telefone { get; set; }

        [LogicalAttribute("statecode")]
        public int? State { get; set; }
        #endregion
    }
}
