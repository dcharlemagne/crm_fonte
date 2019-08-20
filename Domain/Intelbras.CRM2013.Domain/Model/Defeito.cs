using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_defeito")]
    public class Defeito : DomainBase
    {
        private RepositoryService RepositoryService { get; set; }

        public Defeito() { }

        public Defeito(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Defeito(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        [LogicalAttribute("new_name")]
        public String Nome { get; set; }
    }
}
