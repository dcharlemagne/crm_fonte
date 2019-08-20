using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("sharepointdocumentlocation")]
    public class SharePointDocumentLocation : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public SharePointDocumentLocation() { }

        public SharePointDocumentLocation(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public SharePointDocumentLocation(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("sharepointdocumentlocationid")]
        public Guid? ID { get; set; }
              
        [LogicalAttribute("name")]
        public String Nome { get; set; }

        [LogicalAttribute("regardingobjectid")]
        public Lookup Regardingobjectid { get; set; }

        #endregion
    }
}
