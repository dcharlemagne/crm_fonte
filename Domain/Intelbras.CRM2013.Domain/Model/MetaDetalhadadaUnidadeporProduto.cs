using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_metadetalhadadoproduto")]
    public class MetaDetalhadadaUnidadeporProduto : DomainBase
    {
        #region Construtores
        public MetaDetalhadadaUnidadeporProduto(string organization, bool isOffline)
            : base(organization, isOffline)
        {
        }
        public MetaDetalhadadaUnidadeporProduto(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_metadetalhadadoprodutoid")]
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



        // Decimal
        [LogicalAttribute("itbc_MetaPlanejada")]
        public decimal? MetaPlanejada { get; set; }

        [LogicalAttribute("itbc_QtdePlanejada")]
        public decimal? QtdePlanejada { get; set; }

        [LogicalAttribute("itbc_MetaRealizada")]
        public decimal? MetaRealizada { get; set; }

        [LogicalAttribute("itbc_QtdeRealizada")]
        public decimal? QtdeRealizada { get; set; }



        // String
        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }

        

        // Lookup
        [LogicalAttribute("itbc_unidade_negociosid")]
        public Lookup UnidadeNegocio { get; set; }

        [LogicalAttribute("itbc_MetadoProdutoId")]
        public Lookup MetadoProduto { get; set; }

        [LogicalAttribute("itbc_ProdutoId")]
        public Lookup Produto { get; set; }

        #endregion
    }
}