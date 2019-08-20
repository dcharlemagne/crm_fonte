using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_orcamentodocanalporsegmento")]
    public class OrcamentodoCanalporSegmento : DomainBase
    {
        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public OrcamentodoCanalporSegmento(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public OrcamentodoCanalporSegmento(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_orcamentodocanalporsegmentoid")]
        public Guid? ID { get; set; }
        
        [LogicalAttribute("itbc_ano")]
        public int? Ano { get; set; }
        
        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }
        
        [LogicalAttribute("itbc_OrcamentodoCanalId")]
        public Lookup OrcamentodoCanal { get; set; }
        
        [LogicalAttribute("itbc_orcamentoparanovosprodutos")]
        public decimal? OrcamentoParaNovosProdutos { get; set; }
        
        [LogicalAttribute("itbc_OrcamentoPlanejado")]
        public decimal? OrcamentoPlanejado { get; set; }
        
        [LogicalAttribute("itbc_UltAtualizacaoIntegracao")]
        public DateTime? UltAtualizacaoIntegracao { get; set; }
        
        [LogicalAttribute("itbc_Trimestre")]
        public int? Trimestre { get; set; }
        
        [LogicalAttribute("itbc_OrcamentoRealizado")]
        public decimal? OrcamentoRealizado { get; set; }
        
        [LogicalAttribute("itbc_CanalId")]
        public Lookup Canal { get; set; }
        
        [LogicalAttribute("itbc_SegmentoId")]
        public Lookup Segmento { get; set; }
        
        [LogicalAttribute("itbc_unidadedenegocio")]
        public Lookup UnidadedeNegocio { get; set; }
        
        [LogicalAttribute("itbc_IntegradoPor")]
        public String IntegradoPor { get; set; }
        
        [LogicalAttribute("itbc_UsuarioIntegracao")]
        public String UsuarioIntegracao { get; set; }
        
        //[LogicalAttribute("itbc_orcamentodocanalporsegmentoid")]
        //public Guid? ID { get; set; }
        
        //[LogicalAttribute("itbc_Ano")]
        //public int? Ano { get; set; }
        
        //[LogicalAttribute("itbc_name")]
        //public String Nome { get; set; }

        //[LogicalAttribute("itbc_orcamentodocanalid")]
        //public Lookup OrcamentodoCanal { get; set; }
        
        //[LogicalAttribute("itbc_UltAtualizacaoIntegracao")]
        //public DateTime? UltAtualizacaoIntegracao { get; set; }
        
        //[LogicalAttribute("itbc_CanalId")]
        //public Lookup Canal { get; set; }
        
        //[LogicalAttribute("itbc_Trimestre")]
        //public int? Trimestre { get; set; }
        
        //[LogicalAttribute("itbc_OrcamentoRealizado")]
        //public decimal? OrcamentoRealizado { get; set; }
        
        //[LogicalAttribute("itbc_OrcamentoPlanejado")]
        //public decimal? OrcamentoPlanejado { get; set; }

        //[LogicalAttribute("itbc_orcamentoparanovosprodutos")]
        //public decimal? OrcamentoParaNovosProdutos { get; set; }

        //[LogicalAttribute("itbc_SegmentoId")]
        //public Lookup Segmento { get; set; }
        
        //[LogicalAttribute("itbc_unidadedenegocio")]
        //public Lookup UnidadedeNegocio { get; set; }
        
        //[LogicalAttribute("itbc_IntegradoPor")]
        //public String IntegradoPor { get; set; }
        
        //[LogicalAttribute("itbc_UsuarioIntegracao")]
        //public String UsuarioIntegracao { get; set; }
        
        #endregion
    }
}

