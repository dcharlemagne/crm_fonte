using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_questionariopergunta")]
    public class QuestionarioPergunta : IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public QuestionarioPergunta() { }

        public QuestionarioPergunta(string organization, bool isOffline)
                : base(organization, isOffline)
            {
                RepositoryService = new RepositoryService(organization, isOffline);
            }

            public QuestionarioPergunta(string organization, bool isOffline, object provider)
                : base(organization, isOffline, provider)
            {
                RepositoryService = new RepositoryService(organization, isOffline, provider);
            }

        #endregion

        #region Atributos
            [LogicalAttribute("itbc_questionarioperguntaid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("itbc_name")]
            public String Nome { get; set; }

            [LogicalAttribute("itbc_tipo_pergunta")]
            public int? TipoPergunta { get; set; }

            [LogicalAttribute("itbc_ordem")]
            public int? Ordem { get; set; }
        
            [LogicalAttribute("itbc_peso")]
            public decimal? Peso { get; set; }

            [LogicalAttribute("itbc_questionario_id")]
            public Lookup Questionario { get; set; }

            [LogicalAttribute("itbc_grupo")]
            public Lookup Grupo { get; set; }

            [LogicalAttribute("statecode")]
            public int? State { get; set; }
        #endregion
    }
}
