using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_metadokaporproduto")]
    public class PotencialdoKAporProduto : DomainBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public PotencialdoKAporProduto(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public PotencialdoKAporProduto(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_metadokaporprodutoid")]
        public Guid? ID { get; set; }


        // int
        [LogicalAttribute("itbc_ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("itbc_Trimestre")]
        public int? Trimestre { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }


        // Datetime
        [LogicalAttribute("itbc_UltAtualizacaoIntegracao")]
        public DateTime? UltAtualizacaoIntegracao { get; set; }


        // Lookup
        [LogicalAttribute("itbc_MetadoKAporSubfamiliaId")]
        public Lookup PotencialdoKAporSubfamilia { get; set; }

        [LogicalAttribute("itbc_KAouRepresentanteId")]
        public Lookup KAouRepresentante { get; set; }

        [LogicalAttribute("itbc_ProdutoId")]
        public Lookup Produto { get; set; }

        [LogicalAttribute("itbc_unidade_negociosid")]
        public Lookup UnidadeNegocio { get; set; }


        // Decimal
        [LogicalAttribute("itbc_MetaRealizada")]
        public decimal? PotencialRealizado { get; set; }

        [LogicalAttribute("itbc_MetaPlanejada")]
        public decimal? PotencialPlanejado { get; set; }

        [LogicalAttribute("itbc_QtdeRealizada")]
        public decimal? QtdeRealizada { get; set; }

        [LogicalAttribute("itbc_QtdePlanejada")]
        public decimal? QtdePlanejada { get; set; }


        // String
        [LogicalAttribute("itbc_IntegradoPor")]
        public string IntegradoPor { get; set; }

        [LogicalAttribute("itbc_UsuarioIntegracao")]
        public string UsuarioIntegracao { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }

        #endregion
    }
}