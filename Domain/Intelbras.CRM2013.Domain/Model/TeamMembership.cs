using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("teammembership")]
    public class TeamMembership : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public TeamMembership() { }

        public TeamMembership(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public TeamMembership(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("systemuserid")]
        public Guid Usuario { get; set; }

        #endregion
    }
}
