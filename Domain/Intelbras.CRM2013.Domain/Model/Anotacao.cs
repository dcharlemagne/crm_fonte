using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    [LogicalEntity("annotation")]
    public class Anotacao: DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Anotacao() { }

        public Anotacao(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public Anotacao(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("annotationid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("subject")]
        public String Assunto { get; set; }

        [LogicalAttribute("filename")]
        public string NomeArquivos { get; set; }

        [LogicalAttribute("documentbody")]
        public string Body { get; set; }

        [LogicalAttribute("mimetype")]
        public string Tipo { get; set; }

        [LogicalAttribute("NoteText")]
        public string Texto { get; set; }

        [LogicalAttribute("isdocument")]
        public bool TemArquivo { get; set; }

        [LogicalAttribute("objectid")]
        public Lookup EntidadeRelacionada { get; set; }

        #endregion
    }
}

