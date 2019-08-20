using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("annotation")]
    public class Observacao : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Observacao() { }

        public Observacao(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public Observacao(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("annotationid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("subject")]
        public string Assunto { get; set; }

        [LogicalAttribute("notetext")]
        public string Text { get; set; }

        [LogicalAttribute("filename")]
        public string NomeArquivo { get; set; }

        [LogicalAttribute("documentbody")]
        public string Body { get; set; }

        [LogicalAttribute("createdon")]
        public DateTime DataCriacao { get; set; }


        #endregion
    }
}

