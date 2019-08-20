using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_relacionamento")]
    public class Relacionamento : DomainBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public Relacionamento() { }

        public Relacionamento(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Relacionamento(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        private Guid participante1;
        public Guid Participante1
        {
            get { return participante1; }
            set { participante1 = value; }
        }

        private Guid participante2;
        public Guid Participante2
        {
            get { return participante2; }
            set { participante2 = value; }
        }

        private Guid funcao1;
        public Guid Funcao1
        {
            get { return funcao1; }
            set { funcao1 = value; }
        }

        private Guid funcao2;
        public Guid Funcao2
        {
            get { return funcao2; }
            set { funcao2 = value; }
        }

        private string descricao1;
        public string Descricao1
        {
            get { return descricao1; }
            set { descricao1 = value; }
        }

        private string descricao2;
        public string Descricao2
        {
            get { return descricao2; }
            set { descricao2 = value; }
        }

        private string tipoParticipante1;
        public string TipoParticipante1
        {
            get { return tipoParticipante1; }
            set { tipoParticipante1 = value; }
        }

        private string tipoParticipante2;
        public string TipoParticipante2
        {
            get { return tipoParticipante2; }
            set { tipoParticipante2 = value; }
        }

        #endregion

        #region Metodos



        #endregion
    }
}