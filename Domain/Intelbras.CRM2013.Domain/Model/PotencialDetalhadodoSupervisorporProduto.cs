using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_potencialdetalhadosuper_produto")]
    public class PotencialdoSupervisorporProdutoDetalhado : DomainBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public PotencialdoSupervisorporProdutoDetalhado(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public PotencialdoSupervisorporProdutoDetalhado(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_potencialdetalhadosuper_produtoid")]
        public Guid? ID { get; set; }



        // Int
        [LogicalAttribute("itbc_ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("itbc_trimestre")]
        public int? Trimestre { get; set; }

        [LogicalAttribute("itbc_mes")]
        public int? Mes { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }



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



        // Lookup
        [LogicalAttribute("itbc_potencialdetalhadodoSupervisorId")]
        public Lookup PotencialdoSupervisorPorProduto { get; set; }

        [LogicalAttribute("itbc_supervisor")]
        public Lookup Supervisor { get; set; }

        [LogicalAttribute("itbc_produtoId")]
        public Lookup Produto { get; set; }

        [LogicalAttribute("itbc_unidade_negociosid")]
        public Lookup UnidadeNegocio { get; set; }

        #endregion
    }
}