using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_docscanaisextranet")]
    public class DocumentoCanaisExtranet : IntegracaoBase
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public DocumentoCanaisExtranet() { }

        public DocumentoCanaisExtranet(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public DocumentoCanaisExtranet(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("itbc_docscanaisextranetid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_nome")]
        public string Nome { get; set; }

        [LogicalAttribute("itbc_todoscanais")]
        public bool? TodosCanais { get; set; }

        [LogicalAttribute("itbc_vigenciafinal")]
        public DateTime? FimVigencia { get; set; }

        [LogicalAttribute("itbc_vigenciainicio")]
        public DateTime? InicioVigencia { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }

        #endregion
    }
}
