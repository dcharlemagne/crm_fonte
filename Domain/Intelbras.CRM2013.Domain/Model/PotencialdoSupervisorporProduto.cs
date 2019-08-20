using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_potencialdosupervisorporproduto")]
    public class PotencialdoSupervisorporProduto : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public PotencialdoSupervisorporProduto(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public PotencialdoSupervisorporProduto(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("itbc_potencialdosupervisorporprodutoid")]
        public Guid? ID { get; set; }



        // Int
        [LogicalAttribute("itbc_ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("itbc_trimestre")]
        public int? Trimestre { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }



        // Lookup
        [LogicalAttribute("itbc_produtoId")]
        public Lookup Produto { get; set; }

        [LogicalAttribute("itbc_supervisor")]
        public Lookup Supervisor { get; set; }

        [LogicalAttribute("itbc_potencialsupervisorporprodutoId")]
        public Lookup PotencialdoSupervisorPorProduto { get; set; }

        [LogicalAttribute("itbc_unidade_negociosid")]
        public Lookup UnidadeNegocio { get; set; }

        public Lookup Subfamilia { get; set; }



        // String
        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }



        // Decimal
        [LogicalAttribute("itbc_qtdeplanejada")]
        public decimal? QtdePlanejada { get; set; }

        [LogicalAttribute("itbc_potencialplanejado")]
        public decimal? PotencialPlanejado { get; set; }

        [LogicalAttribute("itbc_potencialrealizado")]
        public decimal? PotencialRealizado { get; set; }

        [LogicalAttribute("itbc_qtderealizada")]
        public decimal? QtdeRealizada { get; set; }

        #endregion
    }
}