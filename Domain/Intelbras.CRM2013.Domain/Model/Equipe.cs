using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("team")]
    public class Equipe : DomainBase
    {
        #region Construtores

            private RepositoryService RepositoryService { get; set; }

        public Equipe() { }

        public Equipe(string organization, bool isOffline)
                : base(organization, isOffline)
            {
                RepositoryService = new RepositoryService(organization, isOffline);
            }

            public Equipe(string organization, bool isOffline, object provider)
                : base(organization, isOffline, provider)
            {
                RepositoryService = new RepositoryService(organization, isOffline, provider);
            }

        #endregion


        #region Atributos
            [LogicalAttribute("teamid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("businessunitid")]
            public Lookup Businessunit { get; set; }
        
            [LogicalAttribute("name")]
            public String Nome { get; set; }

        #endregion
    }
}
