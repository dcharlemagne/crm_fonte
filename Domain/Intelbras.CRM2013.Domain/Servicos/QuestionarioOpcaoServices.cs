using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class QuestionarioOpcaoServices
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public QuestionarioOpcaoServices(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public QuestionarioOpcaoServices(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos
        public List<QuestionarioOpcao> ListarQuestionarioOpcaoPor(Guid questionarioPerguntaId)
        {
            return RepositoryService.QuestionarioOpcao.ListarQuestionarioOpcaoPor(questionarioPerguntaId);
        }
        public List<QuestionarioOpcao> ListarPorContaId(Guid contaId)
        {
            return RepositoryService.QuestionarioOpcao.ListarPorContaId(contaId);
        }
        public QuestionarioOpcao ObterPor(Guid Id)
        {
            return RepositoryService.QuestionarioOpcao.Retrieve(Id);
        }

        #endregion

    }
}
