using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("")]
    public class PesquisaSatisfacao : DomainBase
    {
        public PesquisaSatisfacao(string organization, bool isOffline) : base(organization, isOffline) {}

        [LogicalAttribute("")]
        public Guid? ID { get; set; }

        [LogicalAttribute("")]
        public String Nome { get; set; }

        [LogicalAttribute("")]
        public Lookup Contato { get; set; }
    }
}
