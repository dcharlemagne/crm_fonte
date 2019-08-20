using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("orderclose")]
    public class FechamentodoPedido : IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public FechamentodoPedido() { }

        public FechamentodoPedido(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public FechamentodoPedido(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region propertys
        [LogicalAttribute("salesorderid")]
        public Lookup Pedido { get; set; }
        #endregion
    }
}
