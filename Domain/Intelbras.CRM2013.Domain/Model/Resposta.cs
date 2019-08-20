using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("codek_answer")]
    public class Resposta : DomainBase
    {
        private RepositoryService RepositoryService { get; set; }

        public Resposta() { }

        public Resposta(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Resposta(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        private bool respostaCerta;

        public bool RespostaCerta
        {
            get { return respostaCerta; }
            set { respostaCerta = value; }
        }

        private string proximaQuestao;

        public string ProximaQuestao
        {
            get { return proximaQuestao; }
            set { proximaQuestao = value; }
        }

        private string resposta;

        public string Resposta1
        {
            get { return resposta; }
            set { resposta = value; }
        }

        private string alternativa;

        public string Alternativa
        {
            get { return alternativa; }
            set { alternativa = value; }
        }

        private Guid? respostaId;

        public Guid? RespostaId
        {
            get { return respostaId; }
            set { respostaId = value; }
        }

        private string respostaUsuario;

        public string RespostaUsuario
        {
            get { return respostaUsuario; }
            set { respostaUsuario = value; }
        }


    }
}
