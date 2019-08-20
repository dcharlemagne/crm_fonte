using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_questionarioopcao")]
    public class QuestionarioOpcao:IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public QuestionarioOpcao() { }

        public QuestionarioOpcao(string organization, bool isOffline)
                : base(organization, isOffline)
            {
                RepositoryService = new RepositoryService(organization, isOffline);
            }

            public QuestionarioOpcao(string organization, bool isOffline, object provider)
                : base(organization, isOffline, provider)
            {
                RepositoryService = new RepositoryService(organization, isOffline, provider);
            }

        #endregion

        #region Atributos
            [LogicalAttribute("itbc_questionarioopcaoid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("itbc_name")]
            public String Nome { get; set; }

            [LogicalAttribute("itbc_ordem")]
            public int? Ordem { get; set; }
        
            [LogicalAttribute("itbc_pontuacao")]
            public decimal Pontuacao { get; set; }

            [LogicalAttribute("itbc_questionario_pergunta_id")]
            public Lookup QuestionarioPergunta { get; set; }
        
            [LogicalAttribute("statecode")]
            public int? State { get; set; }
        #endregion
    }
}
