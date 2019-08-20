using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("email")]
    public class Email : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Email() { }

        public Email(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Email(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("activityid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("from")]
        public Lookup[] De { get; set; }

        [LogicalAttribute("to")]
        public Lookup[] Para { get; set; }
        
        [LogicalAttribute("torecipients")]
        public string ParaEnderecos { get; set; }

        [LogicalAttribute("subject")]
        public string Assunto { get; set; }

        [LogicalAttribute("description")]
        public string Mensagem { get; set; }

        [LogicalAttribute("regardingobjectid")]
        public Lookup ReferenteA { get; set; }

        [LogicalAttribute("directioncode")]
        public bool? Direcao { get; set; }

        [LogicalAttribute("statuscode")]
        public int? StatusCode { get; set; }

        [LogicalAttribute("statecode")]
        public int? StateCode { get; set; }

        [LogicalAttribute("actualend")]
        public DateTime? TerminoReal { get; set; }

        [LogicalAttribute("senton")]
        public DateTime? EnviadoEm { get; set; }

        [LogicalAttribute("modifiedon")]
        public DateTime? ModificadoEm { get; set; }

        public List<AnexoDeEmail> Anexos;

        public Guid ReferenteAId { get; set; }

        public string ReferenteAType { get; set; }

        public string ReferenteAName { get; set; }

        #endregion
    }

    public class AnexoDeEmail
    {
        public Lookup ObjectId { get; set; }

        public string FileName { get; set; }

        public string MimeType { get; set; }

        public string BodyBase64 { get; set; }
    }
}
