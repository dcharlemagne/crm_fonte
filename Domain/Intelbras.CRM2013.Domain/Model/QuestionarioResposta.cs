using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_questionarioresposta")]
    public class QuestionarioResposta:IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public QuestionarioResposta() { }

        public QuestionarioResposta(string organization, bool isOffline)
                : base(organization, isOffline)
            {
                RepositoryService = new RepositoryService(organization, isOffline);
            }

            public QuestionarioResposta(string organization, bool isOffline, object provider)
                : base(organization, isOffline, provider)
            {
                RepositoryService = new RepositoryService(organization, isOffline, provider);
            }

        #endregion

        #region Atributos
            [LogicalAttribute("itbc_questionariorespostaid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("itbc_name")]
            public String Nome { get; set; }

            [LogicalAttribute("itbc_valor")]
            public int? Valor { get; set; }

            [LogicalAttribute("itbc_questionario_opcao_id")]
            public Lookup QuestionarioOpcao { get; set; }

            [LogicalAttribute("itbc_questionario_resposta_conta_id")]
            public Lookup QuestionarioRespostaConta { get; set; }
        
            [LogicalAttribute("statecode")]
            public int? State { get; set; }

        [LogicalAttribute("itbc_questionario_resposta_ocorrencia_id")]
        public Lookup QuestionarioRespostaOcorrencia { get; set; }
        #endregion
    }
}
