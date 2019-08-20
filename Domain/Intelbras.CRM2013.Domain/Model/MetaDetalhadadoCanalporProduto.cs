using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_metadetalhadadocanalporproduto")]
    public class MetaDetalhadadoCanalporProduto : DomainBase
    {
        #region Construtores

        public MetaDetalhadadoCanalporProduto(string organization, bool isOffline) : base(organization, isOffline) { }

        public MetaDetalhadadoCanalporProduto(string organization, bool isOffline, object provider) : base(organization, isOffline, provider) { }        
        
        #endregion
        
        #region Atributos

        [LogicalAttribute("itbc_metadetalhadadocanalporprodutoid")]
        public Guid? ID { get; set; }



        // Int
        [LogicalAttribute("itbc_Ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("itbc_Trimestre")]
        public int? Trimestre { get; set; }

        [LogicalAttribute("itbc_Mes")]
        public int? Mes { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }



        // String
        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }



        // Decimal
        [LogicalAttribute("itbc_MetaPlanejada")]
        public decimal? MetaPlanejada { get; set; }

        [LogicalAttribute("itbc_QtdePlanejada")]
        public decimal? QtdePlanejada { get; set; }
        
        [LogicalAttribute("itbc_MetaRealizada")]
        public decimal? MetaRealizada { get; set; }
        
        [LogicalAttribute("itbc_QtdeRealizada")]
        public decimal? QtdeRealizada { get; set; }



        // Lookup
        [LogicalAttribute("itbc_metadocanalid")]
        public Lookup MetadoCanal { get; set; }

        [LogicalAttribute("itbc_meta_canal_trimestreid")]
        public Lookup MetadoCanalporTrimestre { get; set; }

        [LogicalAttribute("itbc_MetadoCanalporProdutoId")]
        public Lookup MetadoCanalporProduto { get; set; }

        [LogicalAttribute("itbc_unidade_negocio")]
        public Lookup UnidadedeNegocio { get; set; }

        [LogicalAttribute("itbc_produtoid")]
        public Lookup Produto { get; set; }

        [LogicalAttribute("itbc_CanalId")]
        public Lookup Canal { get; set; }

        #endregion
    }
}