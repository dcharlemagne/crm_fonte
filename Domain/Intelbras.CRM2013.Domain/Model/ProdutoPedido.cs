using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("salesorderdetail")]
    public class ProdutoPedido : IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ProdutoPedido() { }

        public ProdutoPedido(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public ProdutoPedido(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("salesorderdetailid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("baseamount")]
        public Decimal? TabelaPrecoPadrao { get; set; }

        [LogicalAttribute("salesorderid")]
        public Lookup Pedido { get; set; }

        [LogicalAttribute("description")]
        public String Descricao { get; set; }

        [LogicalAttribute("exchangerate")]
        public Decimal? TaxaCambio { get; set; }

        [LogicalAttribute("extendedamount")]
        public Decimal? ValorTotal { get; set; }

        [LogicalAttribute("ispriceoverridden")]
        public Boolean Precificacao { get; set; }

        [LogicalAttribute("isproductoverridden")]
        public Boolean SelecionarProduto { get; set; }

        [LogicalAttribute("itbc_aliquota_ipi")]
        public Decimal? AliquotaIPI { get; set; }

        [LogicalAttribute("itbc_chave_integracao")]
        public String ChaveIntegracao { get; set; }

        [LogicalAttribute("itbc_data_alteracao")]
        public DateTime? DataAlteracao { get; set; }

        [LogicalAttribute("itbc_data_cancelamento_usuario")]
        public DateTime? DataCancelamentoUsuario { get; set; }

        [LogicalAttribute("itbc_data_cancelamentoseq")]
        public DateTime? DataCancelamentoSequencia { get; set; }

        [LogicalAttribute("itbc_data_devolucao")]
        public DateTime? DataDevolucao { get; set; }

        [LogicalAttribute("itbc_data_devolucao_usuario")]
        public DateTime? DataDevolucaoUsuario { get; set; }

        [LogicalAttribute("itbc_data_entrega")]
        public DateTime? DataEntrega { get; set; }

        [LogicalAttribute("itbc_data_entrega_original")]
        public DateTime? DataEntregaOriginal { get; set; }

        [LogicalAttribute("itbc_data_implantacao")]
        public DateTime? DataImplantacao { get; set; }

        [LogicalAttribute("itbc_data_maxima_faturamento")]
        public DateTime? DataMaximaFaturamento { get; set; }

        [LogicalAttribute("itbc_data_minima_faturamento")]
        public DateTime? DataMinimaFaturamento { get; set; }

        [LogicalAttribute("itbc_data_reativacao")]
        public DateTime? DataReativacao { get; set; }

        [LogicalAttribute("itbc_data_reativacao_usuario")]
        public DateTime? DataReativacaoUsuario { get; set; }

        [LogicalAttribute("itbc_data_suspensao")]
        public DateTime? DataSuspensao { get; set; }

        [LogicalAttribute("itbc_data_suspensao_usuario")]
        public DateTime? DataSuspensaoUsuario { get; set; }

        [LogicalAttribute("itbc_descricao")]
        public String Descricaoid { get; set; }

        [LogicalAttribute("itbc_descricao_cancelamento")]
        public String DescricaoCancelamento { get; set; }

        [LogicalAttribute("itbc_descricao_devolucao")]
        public String DescricaoDevolucao { get; set; }

        [LogicalAttribute("itbc_fatura_quantidade_familia")]
        public Boolean FaturaQtdeFamilia { get; set; }

        [LogicalAttribute("itbc_kaourepresentante")]
        public Lookup Representante { get; set; }

        [LogicalAttribute("itbc_natureza_operacao")]
        public Lookup NaturezaOperacao { get; set; }

        [LogicalAttribute("itbc_nome_abreviado")]
        public String NomeAbreviado { get; set; }

        [LogicalAttribute("itbc_observacao")]
        public String Observacao { get; set; }

        [LogicalAttribute("itbc_pedido_cliente")]
        public String PedidoCliente { get; set; }

        [LogicalAttribute("itbc_percentual_desconto_icms")]
        public Decimal? PercentualDescontoICMS { get; set; }

        [LogicalAttribute("itbc_percentual_min_faturamento")]
        public Decimal? PercentualMinimoFaturamento { get; set; }

        [LogicalAttribute("itbc_preco_minimo")]
        public Decimal? PrecoMinimo { get; set; }

        [LogicalAttribute("itbc_preco_negociado")]
        public Decimal? PrecoNegociado { get; set; }

        [LogicalAttribute("itbc_quantidade_alocada")]
        public Decimal? QtdeAlocada { get; set; }

        [LogicalAttribute("itbc_quantidade_alocada_logica")]
        public Decimal? QtdeAlocadaLogica { get; set; }

        [LogicalAttribute("itbc_quantidade_devolvida")]
        public Decimal? QtdeDevolvida { get; set; }

        [LogicalAttribute("itbc_retem_icms_fonte")]
        public Boolean RetemICMSFonte { get; set; }

        [LogicalAttribute("itbc_situacao_alocacao")]
        public Int32? SituacaoAlocacao { get; set; }

        [LogicalAttribute("itbc_situacao_item")]
        public Int32? SituacaoItem { get; set; }

        [LogicalAttribute("itbc_tabeladepreco")]
        public Lookup TabelaPreco { get; set; }

        [LogicalAttribute("itbc_tipo_preco")]
        public Int32? TipoPreco { get; set; }

        [LogicalAttribute("itbc_unidadedenegocio")]
        public Lookup UnidadeNegocio { get; set; }

        [LogicalAttribute("itbc_usuario_alteracao")]
        public String UsuarioAlteracao { get; set; }

        [LogicalAttribute("itbc_usuario_cancelamento")]
        public String UsuarioCancelamento { get; set; }

        [LogicalAttribute("itbc_usuario_devolucao")]
        public String UsuarioDevolucao { get; set; }

        [LogicalAttribute("itbc_usuario_implantacao")]
        public String UsuarioImplantacao { get; set; }

        [LogicalAttribute("itbc_usuario_reativacao")]
        public String UsuarioReativacao { get; set; }

        [LogicalAttribute("itbc_usuario_suspensao")]
        public String UsuarioSuspensao { get; set; }

        [LogicalAttribute("itbc_valor_liquido_aberto")]
        public Decimal? ValorLiquidoAberto { get; set; }

        [LogicalAttribute("itbc_valor_liquido_item")]
        public Decimal? ValorLiquidoItem { get; set; }

        [LogicalAttribute("itbc_valor_mercadoria_aberto")]
        public Decimal? ValorMercadoriaAberto { get; set; }

        [LogicalAttribute("itbc_valor_original")]
        public Decimal? ValorOriginal { get; set; }

        [LogicalAttribute("itbc_valor_tabela")]
        public Decimal? ValorTabela { get; set; }

        [LogicalAttribute("itbc_valor_total_item")]
        public Decimal? ValorTotalItem { get; set; }

        [LogicalAttribute("itbc_valordast")]
        public Decimal? ValorSubstTributaria { get; set; }

        [LogicalAttribute("itbc_valordoipi")]
        public Decimal? ValorIPI { get; set; }

        [LogicalAttribute("manualdiscountamount")]
        public Decimal? DescontoManual { get; set; }

        [LogicalAttribute("productdescription")]
        public String ProdutoForaCatalogo { get; set; }

        [LogicalAttribute("productid")]
        public Lookup Produto { get; set; }

        public Domain.Model.Product ProdutoModel { get; set; }

        [LogicalAttribute("quantity")]
        public Decimal? Quantidade { get; set; }

        [LogicalAttribute("quantitybackordered")]
        public Decimal? QtdePedidoPendente { get; set; }

        [LogicalAttribute("quantitycancelled")]
        public Decimal? QtdeCancelada { get; set; }

        [LogicalAttribute("quantityshipped")]
        public Decimal? QtdeEntregue { get; set; }

        [LogicalAttribute("priceperunit")]
        public Decimal? ValorLiquidoSemIpiSt { get; set; }

        [LogicalAttribute("requestdeliveryby")]
        public DateTime? DateEntregaSolicitada { get; set; }

        [LogicalAttribute("salesrepid")]
        public Lookup Vendedor { get; set; }

        [LogicalAttribute("sequencenumber")]
        public Int32? NumeroSequencia { get; set; }

        [LogicalAttribute("shipto_city")]
        public String CidadeEntrega { get; set; }

        [LogicalAttribute("shipto_contactname")]
        public String NomeContatoEntrega { get; set; }

        [LogicalAttribute("shipto_country")]
        public String PaisEntrega { get; set; }

        [LogicalAttribute("shipto_fax")]
        public String FAXEntrega { get; set; }

        [LogicalAttribute("shipto_freighttermscode")]
        public Int32? CondicoesFrete { get; set; }

        [LogicalAttribute("shipto_line1")]
        public String RuaEntrega { get; set; }

        [LogicalAttribute("shipto_line2")]
        public String BairroEntrega { get; set; }

        [LogicalAttribute("shipto_line3")]
        public String ComplementoEntrega { get; set; }

        [LogicalAttribute("shipto_name")]
        public String NomeEntrega { get; set; }

        [LogicalAttribute("shipto_postalcode")]
        public String CEPEntrega { get; set; }

        [LogicalAttribute("shipto_stateorprovince")]
        public String EstadoEntrega { get; set; }

        [LogicalAttribute("shipto_telephone")]
        public String TelefoneEntrega { get; set; }

        [LogicalAttribute("tax")]
        public Decimal? TotalImposto { get; set; }

        [LogicalAttribute("transactioncurrencyid")]
        public Lookup Moeda { get; set; }

        [LogicalAttribute("uomid")]
        public Lookup Unidade { get; set; }

        [LogicalAttribute("volumediscountamount")]
        public Decimal? DescontoVolume { get; set; }

        [LogicalAttribute("willcall")]
        public Boolean Remessa { get; set; }

        [LogicalAttribute("itbc_calcularrebate")]
        public bool? CalcularRebate { get; set; }

        public String Acao { get; set; }

        [LogicalAttribute("itbc_reproriginal")]
        public String RepresentanteOriginal { get; set; }

        [LogicalAttribute("itbc_errointegrrepresent")]
        public Boolean? IntegradoRepresentanteComErro { get; set; }

        #endregion

        #region Metodos
        public void CancelarParaSoma()
        {
            string comentarioHist = String.Format(
               "Valor Liquido Orig.: {0} | Quantidade: {1} | Total de Imposto: {2}",
               this.ValorLiquidoSemIpiSt,
               this.Quantidade,
               this.TotalImposto
            );

            this.Observacao = comentarioHist + Environment.NewLine + this.Observacao;
            this.Observacao = Helper.Truncate(this.Observacao, 2000);
            this.removeNullProperty("Observacao");
            this.ValorLiquidoSemIpiSt = 0;
            this.Quantidade = 0;
            this.TotalImposto = 0;
        }
        #endregion
    }
}
