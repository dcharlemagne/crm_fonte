using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("codek_livechat_tracking")]
    public class LiveChatTracking : DomainBase
    {
        #region Construtor

        private RepositoryService RepositoryService { get; set; }

        public LiveChatTracking() { }

        public LiveChatTracking(string organization, bool isOffline) : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public LiveChatTracking(string organization, bool isOffline, object provider) : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos   
        [LogicalAttribute("activityid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("subject")]
        public string Subject { get; set; }

        [LogicalAttribute("codek_name_visitor")]
        public string NameVisitor { get; set; }

        [LogicalAttribute("codek_contactid")]
        public Lookup Contact { get; set; }

        [LogicalAttribute("regardingobjectid")]
        public Lookup ReferenteA { get; set; }

        [LogicalAttribute("codek_phone_visitor")]
        public string PhoneVisitor { get; set; }

        [LogicalAttribute("codek_document_visitor")]
        public string DocumentVisitor { get; set; }

        [LogicalAttribute("codek_email_visitor")]
        public string EmailVisitor { get; set; }

        [LogicalAttribute("codek_ip_visitor")]
        public string IPVisitor { get; set; }

        [LogicalAttribute("ownerid")]
        public Lookup Owner { get; set; }

        public StatusLiveChatTracking StatusLiveChatTracking
        {
            get { return (RazaoStatus.HasValue ? (StatusLiveChatTracking)RazaoStatus.Value : StatusLiveChatTracking.Vazio); }
            set { }
        }
        #endregion

        #region Metodos

        #endregion
    }
}