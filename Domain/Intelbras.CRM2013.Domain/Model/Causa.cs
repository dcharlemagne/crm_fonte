using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_causa")]
    public class Causa : DomainBase
    {
        #region Construtor
        private RepositoryService RepositoryService { get; set; }

        public Causa() { }

        public Causa(string organization, bool isOffline) : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Causa(string organization, bool isOffline, object provider) : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("new_causaid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("new_codigo")]
        public String Codigo { get; set; }

        [LogicalAttribute("new_name")]
        public String Nome { get; set; }
        #endregion

        #region Metodos

        #endregion
    }
}