using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("queueitem")]
    public class ItemFila : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ItemFila() { }

        public ItemFila(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ItemFila(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("queueitemid")]
        public Guid? ID { get; set; }        
               
        [LogicalAttribute("queueid")]
        public Lookup Fila { get; set; }

        [LogicalAttribute("objectid")]
        public Lookup Objeto { get; set; }

        [LogicalAttribute("objecttypecode")]
        public Lookup Tipo { get; set; }

        #endregion
    }
}
