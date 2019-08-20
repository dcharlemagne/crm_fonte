using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("appointment")]
    public class Compromisso : DomainBase
    {
        private RepositoryService RepositoryService { get; set; }

        public Compromisso() { }

        public Compromisso(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Compromisso(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #region Atributos

        private DateTime? inicio;
        [LogicalAttribute("actualstart")]
        public DateTime? Inicio
        {
            get { return inicio; }
            set { inicio = value; }
        }
        
        private DateTime? fim;
        [LogicalAttribute("actualend")]
        public DateTime? Fim
        {
            get { return fim; }
            set { fim = value; }
        }
        [LogicalAttribute("actualdurationminutes")]
        private int? duracao;
        public int? Duracao
        {
            get { return duracao; }
            set { duracao = value; }
        }
        
        private string descricao;
        [LogicalAttribute("description")]
        public string Descricao
        {
            get { return descricao; }
            set { descricao = value; }
        }

        private string assunto;
        [LogicalAttribute("subject")]
        public string Assunto
        {
            get { return assunto; }
            set { assunto = value; }
        }

        private Guid referenteAId;
        [LogicalAttribute("regardingobjectid")]
        public Guid ReferenteAId
        {
            get { return referenteAId; }
            set { referenteAId = value; }
        }

        private string referenteAType;
        public string ReferenteAType
        {
            get { return referenteAType; }
            set { referenteAType = value; }
        }

        private string referenteAName;
        public string ReferenteAName
        {
            get { return referenteAName; }
            set { referenteAName = value; }
        }
        #endregion

    }
}
