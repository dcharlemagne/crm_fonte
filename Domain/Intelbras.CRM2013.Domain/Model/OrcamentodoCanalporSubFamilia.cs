using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_orcdocanalporsubfamilia")]
    public class OrcamentodoCanalporSubFamilia : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public OrcamentodoCanalporSubFamilia(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public OrcamentodoCanalporSubFamilia(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_orcdocanalporsubfamiliaid")]
        public Guid? ID { get; set; }
        
        [LogicalAttribute("itbc_OrcamentoPlanejado")]
        public decimal? OrcamentoPlanejado { get; set; }
        
        [LogicalAttribute("itbc_FamiliaId")]
        public Lookup FamiliadeProduto { get; set; }
        
        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }
        
        [LogicalAttribute("itbc_OrcamentoParaNovosProdutos")]
        public decimal? OrcamentoParaNovosProdutos { get; set; }
        
        [LogicalAttribute("itbc_Trimestre")]
        public int? Trimestre { get; set; }
        
        [LogicalAttribute("itbc_OrcamentodoCanalporFamiliaId")]
        public Lookup OrcamentodoCanalporFamilia { get; set; }
        
        [LogicalAttribute("itbc_UltAtualizacaoIntegracao")]
        public DateTime? UltAtualizacaoIntegracao { get; set; }
        
        [LogicalAttribute("itbc_SubFamiliaId")]
        public Lookup SubFamilia { get; set; }
        
        [LogicalAttribute("itbc_OrcamentoRealizado")]
        public decimal? OrcamentoRealizado { get; set; }
        
        [LogicalAttribute("itbc_CanalId")]
        public Lookup Canal { get; set; }
        
        [LogicalAttribute("itbc_UnidadedeNegocioId")]
        public Lookup UnidadedeNegocio { get; set; }
        
        [LogicalAttribute("itbc_SegmentoId")]
        public Lookup Segmento { get; set; }
        
        [LogicalAttribute("itbc_Ano")]
        public int? Ano { get; set; }
        
        [LogicalAttribute("itbc_IntegradoPor")]
        public String IntegradoPor { get; set; }
        
        [LogicalAttribute("itbc_UsuarioIntegracao")]
        public String UsuarioIntegracao { get; set; }
        
        //[LogicalAttribute("itbc_orcdocanalporsubfamiliaid")]
        //public Guid? ID { get; set; }
        
        //[LogicalAttribute("itbc_Ano")]
        //public int? Ano { get; set; }
        
        //[LogicalAttribute("itbc_name")]
        //public String Nome { get; set; }
        
        //[LogicalAttribute("itbc_FamiliaId")]
        //public Lookup FamiliadeProduto { get; set; }
        
        //[LogicalAttribute("itbc_OrcamentoRealizado")]
        //public decimal? OrcamentoRealizado { get; set; }
        
        //[LogicalAttribute("itbc_Trimestre")]
        //public int? Trimestre { get; set; }
        
        //[LogicalAttribute("itbc_UltAtualizacaoIntegracao")]
        //public DateTime? UltAtualizacaoIntegracao { get; set; }
        
        //[LogicalAttribute("itbc_SubFamiliaId")]
        //public Lookup SubFamilia { get; set; }
        
        //[LogicalAttribute("itbc_CanalId")]
        //public Lookup Canal { get; set; }
        
        //[LogicalAttribute("itbc_UnidadedeNegocioId")]
        //public Lookup UnidadedeNegocio { get; set; }
        
        //[LogicalAttribute("itbc_OrcamentoPlanejado")]
        //public decimal? OrcamentoPlanejado { get; set; }

        //[LogicalAttribute("itbc_OrcamentoParaNovosProdutos")]
        //public decimal? OrcamentoParaNovosProdutos { get; set; }
        
        //[LogicalAttribute("itbc_SegmentoId")]
        //public Lookup Segmento { get; set; }
        
        //[LogicalAttribute("itbc_OrcamentodoCanalporFamiliaId")]
        //public Lookup OrcamentodoCanalporFamilia { get; set; }

        //[LogicalAttribute("itbc_IntegradoPor")]
        //public String IntegradoPor { get; set; }
        
        //[LogicalAttribute("itbc_UsuarioIntegracao")]
        //public String UsuarioIntegracao { get; set; }
        
        #endregion
    }
}

