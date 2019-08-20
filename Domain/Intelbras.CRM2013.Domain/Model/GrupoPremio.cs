using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_grupo_premio_fidelidade")]
    public class GrupoPremio: DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public GrupoPremio() { }

        public GrupoPremio(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public GrupoPremio(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }


        #endregion

        [LogicalAttribute("new_name")]
        public String Nome { get; set; }

    }
}
