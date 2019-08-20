using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_questionario_grupo_pergunta")]
    public class QuestionarioGrupoPergunta: DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public QuestionarioGrupoPergunta() { }

        public QuestionarioGrupoPergunta(string organization, bool isOffline)
                : base(organization, isOffline)
            {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public QuestionarioGrupoPergunta(string organization, bool isOffline, object provider)
                : base(organization, isOffline, provider)
            {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("itbc_questionario_grupo_perguntaid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_ordem")]
        public int? Ordem { get; set; }
        #endregion
    }
}
