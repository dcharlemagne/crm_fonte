using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_metadetalhadadokaporproduto")]
    public class MetaDetalhadadoKAporProduto : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public MetaDetalhadadoKAporProduto(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public MetaDetalhadadoKAporProduto(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_metadetalhadadokaporprodutoid")]
        public Guid? ID { get; set; }

        

        // Lookup
        [LogicalAttribute("itbc_ProdutoId")]
        public Lookup Produto { get; set; }

        [LogicalAttribute("itbc_MetadoKAporProdutoId")]
        public Lookup MetadoKAporProduto { get; set; }

        [LogicalAttribute("itbc_KAouRepresentanteId")]
        public Lookup KAouRepresentante { get; set; }



        // String
        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }



        // Decimal
        [LogicalAttribute("itbc_QtdeRealizada")]
        public decimal? QtdeRealizada { get; set; }

        [LogicalAttribute("itbc_MetaPlanejada")]
        public decimal? MetaPlanejada { get; set; }

        [LogicalAttribute("itbc_QtdePlanejada")]
        public decimal? QtdePlanejada { get; set; }

        [LogicalAttribute("itbc_MetaRealizada")]
        public decimal? MetaRealizada { get; set; }



        // Int
        [LogicalAttribute("itbc_Trimestre")]
        public int? Trimestre { get; set; }

        [LogicalAttribute("itbc_ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("itbc_Mes")]
        public int? Mes { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }

        #endregion
    }
}

