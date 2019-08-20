using Intelbras.CRM2013.Domain.Model;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class QuestionarioGrupoPerguntaServices
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public QuestionarioGrupoPerguntaServices(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public QuestionarioGrupoPerguntaServices(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos
        public QuestionarioGrupoPergunta ListarGrupoById(string Id)
        {
            var grupo = RepositoryService.QuestionarioGrupoPergunta.ListarGrupoById(Id);
            if (grupo.Count > 0)
                return grupo.FirstOrDefault();
            else
                return null;
        }

        #endregion
    }
}
