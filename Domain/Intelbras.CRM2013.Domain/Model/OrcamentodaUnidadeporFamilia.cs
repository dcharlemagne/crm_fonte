using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_orcamentoporfamilia")]
    public class OrcamentodaUnidadeporFamilia : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public OrcamentodaUnidadeporFamilia(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public OrcamentodaUnidadeporFamilia(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_orcamentoporfamiliaid")]
        public Guid? ID { get; set; }



        [LogicalAttribute("itbc_OrcamentoPlanejado")]
        public decimal? OrcamentoPlanejado { get; set; }



        [LogicalAttribute("itbc_Ano")]
        public int? Ano { get; set; }



        [LogicalAttribute("itbc_OrcamentoporSegmento")]
        public Lookup OrcamentoporSegmento { get; set; }



        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }



        [LogicalAttribute("itbc_OrcamentoParaNovosProdutos")]
        public decimal? OrcamentoParaNovosProdutos { get; set; }



        [LogicalAttribute("itbc_OrcamentoNaoAlocado")]
        public decimal? OrcamentoNaoAlocado { get; set; }



        [LogicalAttribute("itbc_UltAtualizacaoIntegracao")]
        public DateTime? UltAtualizacaoIntegracao { get; set; }



        [LogicalAttribute("itbc_Trimestre")]
        public int? Trimestre { get; set; }



        [LogicalAttribute("itbc_FamiliadeProduto")]
        public Lookup FamiliadeProduto { get; set; }



        [LogicalAttribute("itbc_OrcamentoRealizado")]
        public decimal? OrcamentoRealizado { get; set; }



        [LogicalAttribute("itbc_UnidadedeNegocioId")]
        public Lookup UnidadedeNegocio { get; set; }



        [LogicalAttribute("itbc_SegmentoId")]
        public Lookup Segmento { get; set; }



        [LogicalAttribute("itbc_IntegradoPor")]
        public String IntegradoPor { get; set; }



        [LogicalAttribute("itbc_UsuarioIntegracao")]
        public String UsuarioIntegracao { get; set; }



        #endregion
    }
}

