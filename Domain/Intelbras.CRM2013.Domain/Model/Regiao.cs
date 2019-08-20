using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("territory")]
    public class Regiao : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Regiao() { }

        public Regiao(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Regiao(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("territoryid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("managerid")]
        public Lookup Gerente { get; set; }

        [LogicalAttribute("name")]
        public String Nome { get; set; }

        [LogicalAttribute("description")]
        public String Descricao { get; set; }
        #endregion
    }
}
