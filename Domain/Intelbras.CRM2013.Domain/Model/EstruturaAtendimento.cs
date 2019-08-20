using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_estrutura_atendimento")]
    public class EstruturaAtendimento:IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public EstruturaAtendimento() { }

        public EstruturaAtendimento(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public EstruturaAtendimento(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
            [LogicalAttribute("itbc_estrutura_atendimentoid")]
            public Guid? ID { get; set; }
        
            [LogicalAttribute("itbc_accountid")]
            public Lookup Canal{ get; set; }

            [LogicalAttribute("itbc_businessunit")]
            public Lookup UnidadeNegocios { get; set; }

            [LogicalAttribute("itbc_estrutura_atendimento")]
            public int? TipoEstruturaAtendimento { get; set; }

            [LogicalAttribute("itbc_name")]
            public String Nome { get; set; }

            [LogicalAttribute("itbc_possui_estrutura")]
            public Boolean? PossueEstrutura { get; set; }

            [LogicalAttribute("statecode")]
            public int? State { get; set; }


        #endregion
    }
}
