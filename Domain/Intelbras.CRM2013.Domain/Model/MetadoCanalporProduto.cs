using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_metadocanalporproduto")]
    public class MetadoCanalporProduto : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public MetadoCanalporProduto(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public MetadoCanalporProduto(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_metadocanalporprodutoid")]
        public Guid? ID { get; set; }



        // Lookup
        [LogicalAttribute("itbc_metadocanalporsubfamiliaid")]
        public Lookup MetadoCanalporSubFamilia { get; set; }

        [LogicalAttribute("itbc_produtoid")]
        public Lookup Produto { get; set; }

        [LogicalAttribute("itbc_CanalId")]
        public Lookup Canal { get; set; }

        //[LogicalAttribute("itbc_subfamiliaid")]
        //public Lookup SubFamilia { get; set; }

        [LogicalAttribute("itbc_unidade_negocio")]
        public Lookup UnidadedeNegocio { get; set; }



        // Int
        [LogicalAttribute("itbc_Ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("itbc_Trimestre")]
        public int? Trimestre { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }



        // Decimal
        [LogicalAttribute("itbc_MetaPlanejada")]
        public decimal? MetaPlanejada { get; set; }

        [LogicalAttribute("itbc_MetaRealizada")]
        public decimal? MetaRealizada { get; set; }



        // String
        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }
        
        #endregion
    }
}