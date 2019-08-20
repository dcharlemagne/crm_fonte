using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_regional")]
    public class Regional : DomainBase
    {
        private RepositoryService RepositoryService { get; set; }

        public Regional() { }

        public Regional(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Regional(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        [LogicalAttribute("new_regionalid")]
        public Guid? ID { get; set; }
        [LogicalAttribute("new_regional")]
        public String Nome { get; set; }
    }
}
