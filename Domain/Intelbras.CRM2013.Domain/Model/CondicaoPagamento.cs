using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_condicao_pagamento")]
    public class CondicaoPagamento : IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public CondicaoPagamento() { }

        public CondicaoPagamento(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public CondicaoPagamento(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("itbc_condicao_pagamentoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_condicao_pagamento")]
        public int? Codigo { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_parcelas")]
        public int? NumeroParcelas { get; set; }

        [LogicalAttribute("itbc_perc_desconto_pgto")]
        public Decimal? PercDesconto { get; set; }

        [LogicalAttribute("itbc_prazos")]
        public int? Prazos { get; set; }

        [LogicalAttribute("itbc_suppliercard")]
        public Boolean? SupplierCard { get; set; }

        [LogicalAttribute("itbc_utilizadoparab2b")]
        public Boolean? UtilizadoParaB2B { get; set; }

        [LogicalAttribute("itbc_utilizadoparacanais")]
        public Boolean? UtilizadoParaCanais { get; set; }

        [LogicalAttribute("itbc_utilizadoparafornecedores")]
        public Boolean? UtilizadoParaFornecedores { get; set; }

        [LogicalAttribute("itbc_utilizadoparasdcv")]
        public Boolean? UtilizadoParaSdcv { get; set; }

        [LogicalAttribute("Itbc_tabeladefinanciamentoid")]
        public Lookup TabelaFinanciamento { get; set; }

        [LogicalAttribute("Itbc_indiceid")]
        public Lookup Indice { get; set; }

        [LogicalAttribute("itbc_utilizadorevenda")]
        public Boolean? UtilizadoRevenda { get; set; }

        #endregion
    }
}