using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("sharepointsite")]
    public class SharePointSite : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public SharePointSite() { }

        public SharePointSite(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public SharePointSite(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("sharepointsiteid")]
        public Guid? ID { get; set; }
        
        [LogicalAttribute("description")]
        public String Descricao { get; set; }

        [LogicalAttribute("absoluteurl")]
        public String UrlAbsoluta { get; set; }

        //[LogicalAttribute("folderstructureentity")]
        //public int? Folderstructureentity { get; set; }

        [LogicalAttribute("relativeurl")]
        public String UrlRelativa {get; set;}

        [LogicalAttribute("name")]
        public String Nome {get; set;}

        [LogicalAttribute("owningteam")]
        public Lookup EquipeProprietario { get; set; }

        [LogicalAttribute("owningbusinessunit")]
        public Lookup UnidadeNegocioProprietario { get; set; }

        [LogicalAttribute("owninguser")]
        public Lookup UsuarioProprietario { get; set; }

        [LogicalAttribute("parentsite")]
        public Lookup SitePrimario { get; set; }

        [LogicalAttribute("statecode")]
        public int? Status { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }


        
        #endregion
    }
}
