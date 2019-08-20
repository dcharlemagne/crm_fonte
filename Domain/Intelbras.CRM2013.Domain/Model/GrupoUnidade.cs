using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("uomschedule")]
    public class GrupoUnidade : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public GrupoUnidade() { }

        public GrupoUnidade(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public GrupoUnidade(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("uomscheduleid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("description")]
        public String Descricao { get; set; }

        [LogicalAttribute("name")]
        public String Nome { get; set; }

        [LogicalAttribute("statecode")]
        public int? State { get; set; }
        #endregion
    }
}
