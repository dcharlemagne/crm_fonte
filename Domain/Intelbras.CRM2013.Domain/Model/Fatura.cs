using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    [LogicalEntity("invoice")]
    public class Fatura : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Fatura() { }

        public Fatura(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Fatura(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("invoiceid")]
        public Guid? ID { get; set; }

        public Enum.StatusDaNotaFiscal StatusDaNotaFiscal
        {
            get { return (RazaoStatus.HasValue ? (Enum.StatusDaNotaFiscal)RazaoStatus.Value : Enum.StatusDaNotaFiscal.Nenhum); }
            set { }
        }

        [LogicalAttribute("billto_city")]
        public String CidadeCobranca { get; set; }

        [LogicalAttribute("billto_country")]
        public String PaisCobranca { get; set; }

        [LogicalAttribute("billto_fax")]
        public String FaxCobranca { get; set; }

        [LogicalAttribute("billto_line1")]
        public String Rua1Cobranca { get; set; }

        [LogicalAttribute("billto_line2")]
        public String Rua2Cobranca { get; set; }

        [LogicalAttribute("billto_line3")]
        public String Rua3Cobranca { get; set; }

        [LogicalAttribute("billto_name")]
        public String NomeCobranca { get; set; }

        [LogicalAttribute("billto_postalcode")]
        public String CEPCobranca { get; set; }

        [LogicalAttribute("billto_stateorprovince")]
        public String EstadoCobranca { get; set; }

        [LogicalAttribute("billto_telephone")]
        public String TelefoneCobranca { get; set; }

        [LogicalAttribute("customerid")]
        public Lookup ClienteId { get; set; }

        private Domain.Model.Conta clienteModel = null;
        public Domain.Model.Conta Cliente
        {
            get
            {
                if (clienteModel == null && this.Id != Guid.Empty)
                    clienteModel = (new Domain.Servicos.RepositoryService()).Conta.ObterPor(this);
                return clienteModel;

            }
            set
            {
                clienteModel = value;
            }
        }

        [LogicalAttribute("datedelivered")]
        public DateTime? DataEntrega { get; set; }

        //[LogicalAttribute("datedelivered")]
        public DateTime? DataDeFaturamento { get; set; }

        [LogicalAttribute("description")]
        public String Descricao { get; set; }

        [LogicalAttribute("discountamount")]
        public Decimal? ValorDescontoFatura { get; set; }

        [LogicalAttribute("discountpercentage")]
        public Decimal? DescontoGlobalTotal { get; set; }

        [LogicalAttribute("duedate")]
        public DateTime? DataConclusao { get; set; }

        [LogicalAttribute("freightamount")]
        public Decimal? ValorFrete { get; set; }

        [LogicalAttribute("invoicenumber")]
        public String IDFatura { get; set; }

        [LogicalAttribute("ispricelocked")]
        public Boolean? PrecosBloqueados { get; set; }

        [LogicalAttribute("itbc_base_substituicao")]
        public Decimal? BaseSubstTributaria { get; set; }

        [LogicalAttribute("itbc_baseicms")]
        public Decimal? BaseICMS { get; set; }

        [LogicalAttribute("itbc_chave_integracao")]
        public String ChaveIntegracao { get; set; }

        [LogicalAttribute("itbc_condicao_pagamento")]
        public Lookup CondicaoPagamento { get; set; }
        public CondicaoPagamento CondicaoDePagamento
        {
            get { return (CondicaoPagamento != null && this.Id != Guid.Empty ? (new CRM2013.Domain.Servicos.RepositoryService()).CondicaoPagamento.Retrieve(this.CondicaoPagamento.Id) : null); }
            set { }
        }

        [LogicalAttribute("itbc_cpfoucnpj")]
        public String CpfCnpj { get; set; }

        [LogicalAttribute("itbc_data_cancelamento")]
        public DateTime? DataCancelamento { get; set; }

        [LogicalAttribute("itbc_data_confirmacao")]
        public DateTime? DataConfirmacao { get; set; }

        [LogicalAttribute("itbc_data_emissao")]
        public DateTime? DataEmissao { get; set; }

        [LogicalAttribute("itbc_data_saida")]
        public DateTime? DataSaida { get; set; }

        [LogicalAttribute("itbc_endereodeentreganr")]
        public String EnderecoEntregaNumero { get; set; }

        [LogicalAttribute("itbc_estabelecimento")]
        public Lookup Estabelecimento { get; set; }

        [LogicalAttribute("itbc_frete")]
        public String Frete { get; set; }

        [LogicalAttribute("itbc_inscricao_estadual")]
        public String InscricaoEstadual { get; set; }

        [LogicalAttribute("itbc_kaourepresentanteid")]
        public Lookup KARepresentante { get; set; }

        [LogicalAttribute("itbc_natureza_operacao")]
        public Lookup NaturezaOperacao { get; set; }

        [LogicalAttribute("itbc_nome_abreviado")]
        public String NomeAbreviado { get; set; }

        [LogicalAttribute("itbc_observacao")]
        public String Observacao { get; set; }

        [LogicalAttribute("itbc_pedido_cliente")]
        public String PedidoCliente { get; set; }

        [LogicalAttribute("itbc_peso_bruto")]
        public Decimal? PesoBruto { get; set; }

        [LogicalAttribute("itbc_peso_liquido")]
        public Decimal? PesoLiquido { get; set; }

        [LogicalAttribute("itbc_serie")]
        public String Serie { get; set; }

        [LogicalAttribute("itbc_shipto_city")]
        public Lookup EnderecoEntregaCidade { get; set; }

        [LogicalAttribute("itbc_shipto_country")]
        public Lookup EnderecoEntregaPais { get; set; }

        [LogicalAttribute("itbc_shipto_postofficebox")]
        public String EnderecoEntregaCaixaPostal { get; set; }

        [LogicalAttribute("itbc_shipto_stateorprovince")]
        public Lookup EnderecoEntregaEstado { get; set; }

        [LogicalAttribute("itbc_shipto_street")]
        public String EnderecoEntregaRua { get; set; }

        [LogicalAttribute("itbc_transportadora")]
        public Lookup Transportadora { get; set; }

        [LogicalAttribute("itbc_valor_substituicao")]
        public Decimal? ValorSubstituicao { get; set; }

        [LogicalAttribute("itbc_valoricms")]
        public Decimal? ValorICMS { get; set; }

        [LogicalAttribute("itbc_valoripi")]
        public Decimal? ValorIPI { get; set; }

        [LogicalAttribute("itbc_vlortotalprodutossemipiest")]
        public Decimal? ValorTotalProdutosSemIPIST { get; set; }

        [LogicalAttribute("itbc_volume")]
        public String Volume { get; set; }

        [LogicalAttribute("name")]
        public String NumeroNF { get; set; }

        [LogicalAttribute("opportunityid")]
        public Lookup Oportunidade { get; set; }

        [LogicalAttribute("paymenttermscode")]
        public Int32? CondicoesPagamento { get; set; }

        [LogicalAttribute("pricelevelid")]
        public Lookup ListaPrecos { get; set; }

        [LogicalAttribute("prioritycode")]
        public Int32? Prioridade { get; set; }

        [LogicalAttribute("salesorderid")]
        public Lookup PedidoCRM { get; set; }

        [LogicalAttribute("shippingmethodcode")]
        public Int32? MetodoEntrega { get; set; }

        [LogicalAttribute("shipto_city")]
        public String CidadeEntrega { get; set; }

        [LogicalAttribute("shipto_country")]
        public String PaisEntrega { get; set; }

        [LogicalAttribute("shipto_fax")]
        public String FaxEntrega { get; set; }

        [LogicalAttribute("shipto_freighttermscode")]
        public Int32? CondicoesFrete { get; set; }

        [LogicalAttribute("shipto_line1")]
        public String Rua1Entrega { get; set; }

        [LogicalAttribute("shipto_line2")]
        public String ComplementoEntrega { get; set; }

        [LogicalAttribute("shipto_line3")]
        public String BairroEntrega { get; set; }

        [LogicalAttribute("shipto_name")]
        public String NomeEntrega { get; set; }

        [LogicalAttribute("shipto_postalcode")]
        public String CEPEntrega { get; set; }

        [LogicalAttribute("shipto_stateorprovince")]
        public String EstadoEntrega { get; set; }

        [LogicalAttribute("shipto_telephone")]
        public String TelefoneEntrega { get; set; }

        [LogicalAttribute("totalamount")]
        public Decimal? ValorTotal { get; set; }

        [LogicalAttribute("totalamountlessfreight")]
        public Decimal? ValorTotalSemFrete { get; set; }

        [LogicalAttribute("totaldiscountamount")]
        public Decimal? DescontoTotal { get; set; }

        [LogicalAttribute("totallineitemamount")]
        public Decimal? TotalProdutosComIPIST { get; set; }

        [LogicalAttribute("totaltax")]
        public Decimal? TotalImpostos { get; set; }

        [LogicalAttribute("transactioncurrencyid")]
        public Lookup Moeda { get; set; }

        [LogicalAttribute("willcall")]
        public Boolean? ClienteRetira { get; set; }

        [LogicalAttribute("itbc_reproriginal")]
        public string RepresentanteOriginal { get; set; }

        [LogicalAttribute("itbc_integradocomerros")]
        public Boolean? IntegradoComErros { get; set; }

        [LogicalAttribute("itbc_errointegrrepresent")]
        public Boolean? IntegradoRepresentanteComErro { get; set; }

        [LogicalAttribute("itbc_tipodanotafiscal")]
        public int? TipoNotaFiscal { get; set; }

        [LogicalAttribute("itbc_identifuniconfe")]
        public string IdentificadorUnicoNfe { get; set; }

        //CRM4
        private List<ProdutoFatura> produtos = null;
        public List<ProdutoFatura> Produtos
        {
            get
            {
                if (this.produtos == null && this.Id != Guid.Empty)
                    this.produtos = (new Domain.Servicos.RepositoryService()).ProdutoFatura.ListarProdutosDaFaturaPor(this.Id);

                return this.produtos;
            }
            set { this.produtos = value; }
        }

       // [LogicalAttribute("itbc_identifuniconfe")]
        public string CFOP { get; set; }
        //CRM4


        [LogicalAttribute("itbc_notadevolucao")]
        public Boolean? NotaDevolucao { get; set; }

        #endregion
    }
}