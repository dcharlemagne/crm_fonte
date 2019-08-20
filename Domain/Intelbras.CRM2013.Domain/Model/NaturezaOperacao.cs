using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_natureza_operacao")]
    public class NaturezaOperacao : IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public NaturezaOperacao() { }

        public NaturezaOperacao(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public NaturezaOperacao(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("itbc_natureza_operacaoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_codigo_natureza_operacao")]
        public String Codigo { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_Tipo_natureza_operacao")]
        public int? Tipo { get; set; }

        [LogicalAttribute("itbc_emiteduplicata")]
        public Boolean? EmiteDuplicata { get; set; }

        [LogicalAttribute("itbc_atualizaestatisticas")]
        public Boolean? AtualizarEstatisticas { get; set; }

        [LogicalAttribute("statecode")]
        public int? State { get; set; }
        #endregion
    }
}
