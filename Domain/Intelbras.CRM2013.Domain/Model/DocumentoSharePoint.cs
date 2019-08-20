using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("sharepointdocumentlocation")]
    public class DocumentoSharePoint : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public DocumentoSharePoint() { }

        public DocumentoSharePoint(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public DocumentoSharePoint(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("sharepointdocumentlocationid")]
        public Guid? ID { get; set; }
        
        [LogicalAttribute("description")]
        public String Descricao { get; set; }

        [LogicalAttribute("absoluteurl")]
        public String UrlAbsoluta { get; set; }

        [LogicalAttribute("relativeurl")]
        public String UrlRelativa {get; set;}

        [LogicalAttribute("name")]
        public String Nome {get; set;}

        [LogicalAttribute("regardingobjectid")]
        public Lookup ObjetoRelacionadoId { get; set; }

        [LogicalAttribute("RegardingObjectIdName")]
        public String ObjetoRelacionadoNome { get; set; }

        
        #endregion
    }
}
