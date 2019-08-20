using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("invoicedetail")]
    public class ProdutoFatura:DomainBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public ProdutoFatura() { }

        public ProdutoFatura(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }


        public ProdutoFatura(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("invoicedetailid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("invoiceid")]
        public Lookup Fatura { get; set; }

        [LogicalAttribute("actualdeliveryon")]
        public DateTime? Entregueem { get; set; }

        [LogicalAttribute("baseamount")]
        public Decimal? PrecoTabelaPadrao { get; set; }

        [LogicalAttribute("description")]
        public String Descricao { get; set; }

        [LogicalAttribute("extendedamount")]
        public Decimal? ValorTotal { get; set; }

        [LogicalAttribute("ispriceoverridden")]
        public Boolean Precificacao { get; set; }

        [LogicalAttribute("isproductoverridden")]
        public Boolean SelecionarProduto { get; set; }

        [LogicalAttribute("itbc_aliquota_icms")]
        public Double AliquotaICMS { get; set; }

        [LogicalAttribute("itbc_aliquota_ipi")]
        public Double AliquotaIPI { get; set; }

        [LogicalAttribute("itbc_aliquota_iss")]
        public Double AliquotaISS { get; set; }

        [LogicalAttribute("itbc_chave_integracao")]
        public String ChaveIntegracao { get; set; }

        [LogicalAttribute("itbc_codigo_tributario_icms")]
        public String CodigoTributarioICMS { get; set; }

        [LogicalAttribute("itbc_codigo_tributario_ipi")]
        public String CodigoTributarioIPI { get; set; }

        [LogicalAttribute("itbc_codigo_tributario_iss")]
        public String CodigoTributarioISS { get; set; }

        [LogicalAttribute("itbc_kaourepresentante")]
        public Lookup Representante { get; set; }

        [LogicalAttribute("itbc_natureza_operacao")]
        public Lookup NaturezaOperacao { get; set; }

        [LogicalAttribute("itbc_preco_consumidor")]
        public Decimal? PrecoConsumidor { get; set; }

        [LogicalAttribute("itbc_preco_liquido")]
        public Decimal? PrecoLiquido { get; set; }

        [LogicalAttribute("itbc_preco_original")]
        public Decimal? PrecoOriginal { get; set; }

        [LogicalAttribute("itbc_unidadedenegocio")]
        public Lookup UnidadeNegocio { get; set; }

        [LogicalAttribute("itbc_valor_base_icms")]
        public Decimal? ValorBaseICMS { get; set; }

        [LogicalAttribute("itbc_valor_base_ipi")]
        public Decimal? ValorBaseIPI { get; set; }

        [LogicalAttribute("itbc_valor_base_iss")]
        public Decimal? ValorBaseISS { get; set; }

        [LogicalAttribute("itbc_valor_icms_item")]
        public Decimal? ValorICMSItem { get; set; }

        [LogicalAttribute("itbc_valor_icms_nao_trib")]
        public Decimal? ValorICMSNaoTributado { get; set; }

        [LogicalAttribute("itbc_valor_icms_outras")]
        public Decimal? ValorICMSOutras { get; set; }

        [LogicalAttribute("itbc_valor_icms_subst_trib")]
        public Decimal? ValorICMSSubstTributaria { get; set; }

        [LogicalAttribute("itbc_valor_icms_subst_trib_base")]
        public Decimal? ValorICMSSubstTributariaBase { get; set; }

        [LogicalAttribute("itbc_valor_ipi_item")]
        public Decimal? ValorIPIItem { get; set; }

        [LogicalAttribute("itbc_valor_ipi_nao_trib")]
        public Decimal? ValorIPINaoTributado { get; set; }

        [LogicalAttribute("itbc_valor_ipi_outras")]
        public Decimal? ValorIPIOutras { get; set; }

        [LogicalAttribute("itbc_valor_iss_item")]
        public Decimal? ValorISSItem { get; set; }

        [LogicalAttribute("itbc_valor_iss_nao_trib")]
        public Decimal? ValorISSNaoTributado { get; set; }

        [LogicalAttribute("itbc_valor_iss_outras")]
        public Decimal? ValorISSOutras { get; set; }

        [LogicalAttribute("itbc_valor_mercadoria_liquida")]
        public Decimal? ValorMercadoriaLiquida { get; set; }

        [LogicalAttribute("itbc_valor_mercadoria_original")]
        public Decimal? ValorMercadoriaOriginal { get; set; }

        [LogicalAttribute("itbc_valor_mercadoria_tabela")]
        public Decimal? ValorMercadoriaTabela { get; set; }

        [LogicalAttribute("itbc_valor_original")]
        public Decimal? ValorOriginal { get; set; }

        [LogicalAttribute("itbc_valor_subst_trib")]
        public Decimal? ValorSubstTributaria { get; set; }

        [LogicalAttribute("lineitemnumber")]
        public Int32? NumeroLinhaItem { get; set; }

        [LogicalAttribute("manualdiscountamount")]
        public Decimal? DescontoManual { get; set; }

        [LogicalAttribute("priceperunit")]
        public Decimal? ValorLiquido { get; set; }

        [LogicalAttribute("productdescription")]
        public String DescricaoProdutoSemCatalogo { get; set; }

        [LogicalAttribute("productid")]
        public Lookup ProdutoId { get; set; }
        public Product Produto
        {
            get { return (ProdutoId != null && this.Id != Guid.Empty ? (new CRM2013.Domain.Servicos.RepositoryService()).Produto.Retrieve(this.ProdutoId.Id) : null); }
            set { }
        }

        [LogicalAttribute("quantity")]
        public Decimal? Quantidade { get; set; }

        [LogicalAttribute("quantitybackordered")]
        public Decimal? QtdePedidoPendente { get; set; }

        [LogicalAttribute("quantitycancelled")]
        public Decimal? QtdeCancelada { get; set; }

        [LogicalAttribute("sequencenumber")]
        public Int32? NumeroSequencia { get; set; }

        [LogicalAttribute("quantityshipped")]
        public Decimal? QtdeEntregue { get; set; }

        [LogicalAttribute("shipto_city")]
        public String CidadeEntrega { get; set; }

        [LogicalAttribute("shipto_country")]
        public String PaisEntrega { get; set; }

        [LogicalAttribute("shipto_fax")]
        public String FAXEntrega { get; set; }

        [LogicalAttribute("shipto_freighttermscode")]
        public Int32? CondicoesFrete { get; set; }

        [LogicalAttribute("shipto_line1")]
        public String Rua1Entrega { get; set; }

        [LogicalAttribute("shipto_line2")]
        public String Rua2Entrega { get; set; }

        [LogicalAttribute("shipto_line3")]
        public String Rua3Entrega { get; set; }

        [LogicalAttribute("shipto_name")]
        public String NomeEntrega { get; set; }

        [LogicalAttribute("shipto_postalcode")]
        public String CEPEntrega { get; set; }

        [LogicalAttribute("shipto_stateorprovince")]
        public String EstadoEntrega { get; set; }

        [LogicalAttribute("shipto_telephone")]
        public String TelefoneEntrega { get; set; }

        [LogicalAttribute("tax")]
        public Decimal? TotalImpostos { get; set; }

        [LogicalAttribute("transactioncurrencyid")]
        public Lookup Moeda { get; set; }

        [LogicalAttribute("uomid")]
        public Lookup Unidade { get; set; }

        [LogicalAttribute("volumediscountamount")]
        public Decimal? DescontoVolume { get; set; }

        [LogicalAttribute("willcall")]
        public Boolean Remessa { get; set; }

        [LogicalAttribute("invoicestatecode")]
        public new int? RazaoStatus {get; set;}

        [LogicalAttribute("itbc_calcularrebate")]
        public bool? CalcularRebate { get; set; }

        [LogicalAttribute("itbc_reproriginal")]
        public string RepresentanteOriginal { get; set; }

        [LogicalAttribute("itbc_errointegrrepresent")]
        public Boolean? IntegradoRepresentanteComErro { get; set; }

        #endregion
    }
}
