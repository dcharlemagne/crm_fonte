using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("customerrelationship")]
    public class RelacionamentoCliente : DomainBase
    {
        private RepositoryService RepositoryService { get; set; }


        public RelacionamentoCliente() { }

        public RelacionamentoCliente(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }


        public RelacionamentoCliente(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        [LogicalAttribute("customerid")]
        public Lookup Participante1 { get; set; }

        [LogicalAttribute("customerroleid")]
        public Lookup Funcao1 { get; set; }

        [LogicalAttribute("partnerid")]
        public Lookup Participante2 { get; set; }

        [LogicalAttribute("partnerroleid")]
        public Lookup Funcao2 { get; set; }
    }
}
