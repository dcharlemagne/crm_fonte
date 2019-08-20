using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_metaporproduto")]
    public class MetadaUnidadeporProduto : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public MetadaUnidadeporProduto(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public MetadaUnidadeporProduto(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_metaporprodutoid")]
        public Guid? ID { get; set; }


        // Int
        [LogicalAttribute("itbc_Ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("itbc_trimestre")]
        public int? Trimestre { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }



        // Decimal
        [LogicalAttribute("itbc_MetaPlanejada")]
        public decimal? MetaPlanejada { get; set; }

        [LogicalAttribute("itbc_MetaRealizada")]
        public decimal? MetaRealizada { get; set; }

        [LogicalAttribute("itbc_QtdeRealizada")]
        public decimal? QtdeRealizada { get; set; }

        [LogicalAttribute("itbc_QtdePlanejada")]
        public decimal? QtdePlanejada { get; set; }


        // String 
        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }

        

        // Lookup
        [LogicalAttribute("itbc_unidade_negociosid")]
        public Lookup UnidadeNegocio { get; set; }

        [LogicalAttribute("itbc_SubfamiliaId")]
        public Lookup MetadaSubfamilia { get; set; }

        [LogicalAttribute("itbc_productId")]
        public Lookup Produto { get; set; }

        [LogicalAttribute("itbc_subfamilia_produtoid")]
        public Lookup Subfamilia { get; set; }

        #endregion
    }
}

