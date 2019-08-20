using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_docsrequeridoscanal")]
    public class DocumentoCanal : DomainBase
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public DocumentoCanal() { }

        public DocumentoCanal(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public DocumentoCanal(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("itbc_docsrequeridoscanalid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_accountid")]
        public Lookup Canal { get; set; }

        [LogicalAttribute("itbc_compdocanalid")]
        public Lookup Compromisso { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_tipodocumentoid")]
        public Lookup TipoDocumento { get; set; }

        [LogicalAttribute("itbc_validade")]
        public DateTime? Validade { get; set; }
      
        #endregion
    }
}
