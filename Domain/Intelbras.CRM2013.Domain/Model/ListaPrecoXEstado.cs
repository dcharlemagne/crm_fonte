using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_pricelevel_itbc_estado")]
    public class ListaPrecoXEstado : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ListaPrecoXEstado() { }

        public ListaPrecoXEstado(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ListaPrecoXEstado(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_pricelevel_itbc_estadoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_estadoid")]
        public Guid? EstadoId { get; set; }

        [LogicalAttribute("pricelevelid")]
        public Guid? ListaPrecoId { get; set; }

        #endregion
    }
}
