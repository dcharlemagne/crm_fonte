using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_localidade")] //sem entidade no CRM
    public class LocalidadeContrato : DomainBase
    {
        private RepositoryService RepositoryService { get; set; }

        public LocalidadeContrato() { }

        public LocalidadeContrato(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }


        public LocalidadeContrato(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        //[LogicalAttribute("subject")]
        public String Nome { get; set; }

    }
}
