using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_relacionamentodocanal")]
    public class RelacionamentoCanal : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public RelacionamentoCanal() { }

        public RelacionamentoCanal(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }


        public RelacionamentoCanal(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_relacionamentodocanalid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_accountid")]
        public Lookup Canal { get; set; }

        [LogicalAttribute("itbc_assistenteid")]
        public Lookup Assistente { get; set; }

        [LogicalAttribute("itbc_contactid")]
        public Lookup KeyAccount { get; set; }

        [LogicalAttribute("itbc_datafinal")]
        public DateTime? DataFinal { get; set; }

        [LogicalAttribute("itbc_datainicial")]
        public DateTime? DataInicial { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_supervisorid")]
        public Lookup Supervisor { get; set; }

        [LogicalAttribute("statecode")]
        public int? Status { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }

        [LogicalAttribute("itbc_acaocrm")]
        public Boolean IntegrarNoPlugin { get; set; }

        #endregion
    }
}
