using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class QuestionarioPerguntaServices
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public QuestionarioPerguntaServices(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public QuestionarioPerguntaServices(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos
        public List<QuestionarioPergunta> ListarQuestionarioPerguntaPorQuestionario(string questionarioId)
        {
            return RepositoryService.QuestionarioPergunta.ListarQuestionarioPerguntaPorQuestionario(questionarioId);
        }
        
        public List<QuestionarioPergunta> ListarQuestionarioPerguntaPorNomeQuestionario(string nomeQuestionario)
        {
            return RepositoryService.QuestionarioPergunta.ListarQuestionarioPerguntaPorNomeQuestionario(nomeQuestionario);
        }       

        #endregion

    }
}
