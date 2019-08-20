using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class QuestionarioRespostaServices
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public QuestionarioRespostaServices(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public QuestionarioRespostaServices(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos
        public List<QuestionarioResposta> ListarTodos()
        {
            return RepositoryService.QuestionarioResposta.ListarTodos();
        }
        public QuestionarioResposta ObterPorOpcaoId(Guid opcaoId, string contaId, bool status)
        {
            return RepositoryService.QuestionarioResposta.ObterPorOpcaoId(opcaoId, contaId, status);
        }

        public List<QuestionarioResposta> ObterREspostaConta(string contaId, bool status)
        {
            if (contaId != null)
            {
                return RepositoryService.QuestionarioResposta.ObterRespostasConta(contaId, status);
            }

            List<QuestionarioResposta> QuestionarioResposta = new List<QuestionarioResposta>();

            return QuestionarioResposta;
        }

        public void Desativar(Guid questinarioId)
        {
            //status 1 = desativar. 
            RepositoryService.QuestionarioResposta.AlterarStatus(questinarioId, 1);
        }
        public void Ativar(Guid questinarioId)
        {
            //status 0 = ativar. 
            RepositoryService.QuestionarioResposta.AlterarStatus(questinarioId, 0);
        }
        #endregion

    }
}
