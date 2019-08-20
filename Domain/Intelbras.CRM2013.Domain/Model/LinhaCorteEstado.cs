using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_itbc_linhadecorte_itbc_estado")]
    public class LinhaCorteEstado : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public LinhaCorteEstado() { }

        public LinhaCorteEstado(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public LinhaCorteEstado(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("itbc_itbc_linhadecorte_itbc_estadoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_linhadecorteid")]
        public Guid? LinhaCorteDistribuidor { get; set; }

        [LogicalAttribute("itbc_estadoid")]
        public Guid? Estado { get; set; }

        #endregion
    }
}
