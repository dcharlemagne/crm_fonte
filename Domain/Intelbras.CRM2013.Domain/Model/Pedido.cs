using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    [LogicalEntity("salesorder")]
    public class Pedido : IntegracaoBase
    {
        #region Construtores

         private RepositoryService RepositoryService { get; set; }

        public Pedido() { }

        public Pedido(String organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Pedido(String organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
            [LogicalAttribute("salesorderid")]
            public Guid? ID { get; set; }

            //[LogicalAttribute("billto_city")]
            //public String CidadeCobranca { get; set; }

            //[LogicalAttribute("billto_composite")]
            //public String EnderecoCobranca { get; set; }

            //[LogicalAttribute("billto_contactname")]
            //public String NomeContatoCobranca { get; set; }

            //[LogicalAttribute("billto_country")]
            //public String RegiaoCobranca { get; set; }

            //[LogicalAttribute("billto_fax")]
            //public String FaxCobranca { get; set; }

            //[LogicalAttribute("billto_line1")]
            //public String Rua1Cobranca { get; set; }

            //[LogicalAttribute("billto_line2")]
            //public String Rua2Cobranca { get; set; }

            //[LogicalAttribute("billto_line3")]
            //public String Rua3Cobranca { get; set; }

            //[LogicalAttribute("billto_name")]
            //public String NomeCobranca { get; set; }

            //[LogicalAttribute("billto_postalcode")]
            //public String CEPCobranca { get; set; }

            //[LogicalAttribute("billto_stateorprovince")]
            //public String EstadoCobranca { get; set; }

            //[LogicalAttribute("billto_telephone")]
            //public String TelefoneCobranca { get; set; }

            [LogicalAttribute("campaignid")]    
            public Lookup CampanhaID { get; set; }

            [LogicalAttribute("customerid")]
            public Lookup ClienteID { get; set; }

            [LogicalAttribute("datefulfilled")]
            public DateTime? DataCumprimento { get; set; }

            [LogicalAttribute("description")]
            public String Descricao { get; set; }

            [LogicalAttribute("discountamount")]
            public Decimal? DescontoGlobalAdicional { get; set; }

            [LogicalAttribute("discountpercentage")]
            public Decimal? DescontoPedido { get; set; }

            [LogicalAttribute("freightamount")]
            public Decimal? ValorFrete { get; set; }

            [LogicalAttribute("freighttermscode")]
            public int? CondicoesFrete { get; set; }

            [LogicalAttribute("ispricelocked")]
            public Boolean? PrecoBloqueado { get; set; }

            [LogicalAttribute("itbc_accountid")]
            public Lookup ClienteTriangular { get; set; }

            [LogicalAttribute("itbc_aprovacao_forcado_pedido")]
            public String AprovacaoForcadoPedido { get; set; }

            [LogicalAttribute("itbc_aprovacao_pedido")]
            public Boolean? Aprovacao { get; set; }

            [LogicalAttribute("itbc_aprovador_pedido")]
            public String Aprovador { get; set; }

            [LogicalAttribute("itbc_canaldevenda")]
            public Lookup CanalVendaID { get; set; }

            [LogicalAttribute("itbc_cidade_cif")]
            public String CidadeCIF { get; set; }

            [LogicalAttribute("itbc_classificacao")]
            public Lookup Classificacao { get; set; }

            [LogicalAttribute("itbc_codigo_entrega")]
            public String CodigoEntrega { get; set; }

            [LogicalAttribute("itbc_codigo_entrega_triangular")]
            public String CodigoEntregaTriangular { get; set; }

            [LogicalAttribute("itbc_codigo_situacao_avaliacao")]
            public int? CodigoSituacaoAvaliacao { get; set; }

            [LogicalAttribute("itbc_completo")]
            public Boolean? Completo { get; set; }

            [LogicalAttribute("itbc_condicao_pagamento")]
            public Lookup CondicaoPagamento { get; set; }

            [LogicalAttribute("itbc_condicoesderedespacho")]
            public String CondicoesRedespacho { get; set; }

            [LogicalAttribute("itbc_condicoesespeciais")]
            public String CondicoesEspeciais { get; set; }

            [LogicalAttribute("itbc_cpfoucnpj")]
            public String CPFCNPJ { get; set; }

            [LogicalAttribute("itbc_data_alteracao")]
            public DateTime? DataAlteracao { get; set; }

            [LogicalAttribute("itbc_data_aprovacao")]
            public DateTime? DataAprovacao { get; set; }

            [LogicalAttribute("itbc_data_cancelamento")]
            public DateTime? DataCancelamento { get; set; }

            [LogicalAttribute("itbc_data_cancelamento_usuario")]
            public DateTime? DataCancelamentoUsuario { get; set; }

            [LogicalAttribute("itbc_data_emissao")]
            public DateTime? DataEmissao { get; set; }

            [LogicalAttribute("itbc_data_entrega")]
            public DateTime? DataEntrega { get; set; }

        [LogicalAttribute("itbc_data_entrega_original")]
            public DateTime? DataEntregaOriginal { get; set; }

            [LogicalAttribute("itbc_data_faturamento")]
            public DateTime? DataLimiteFaturamento { get; set; }

            [LogicalAttribute("itbc_data_implantacao")]
            public DateTime? DataImplantacao { get; set; }

            [LogicalAttribute("itbc_data_implantacao_usuario")]
            public DateTime? DataImplantacaoUsuario { get; set; }

            [LogicalAttribute("itbc_data_minima_faturamento")]
            public DateTime? DataMinimaFaturamento { get; set; }

            [LogicalAttribute("itbc_data_negociacao")]
            public DateTime? DataNegociacao { get; set; }

            [LogicalAttribute("itbc_data_reativacao")]
            public DateTime? DataReativacao { get; set; }

            [LogicalAttribute("itbc_data_reativacao_usuario")]
            public DateTime? DataReativacaoUsuario { get; set; }

            [LogicalAttribute("itbc_data_suspensao")]
            public DateTime? DataSuspensao { get; set; }

            [LogicalAttribute("itbc_desconto1")]
            public Decimal? PercentualDesconto1 { get; set; }

            [LogicalAttribute("itbc_desconto2")]
            public Decimal? PercentualDesconto2 { get; set; }

            [LogicalAttribute("itbc_descricao_cancelamento")]
            public String DescricaoCancelamento { get; set; }

            [LogicalAttribute("itbc_descricao_suspensao")]
            public String DescricaoSuspensao { get; set; }

            [LogicalAttribute("itbc_destino_mercadoria")]
            public int? DestinoMercadoria { get; set; }

            [LogicalAttribute("itbc_dias_negociacao")]
            public int? DiasNegociacao { get; set; }

            [LogicalAttribute("itbc_estabelecimento")]
            public Lookup Estabelecimento { get; set; }

            private Estabelecimento estabelecimentoModel = null;
            public Estabelecimento EstabelecimentoModel
            {
                get
                {
                    if (estabelecimentoModel == null && Estabelecimento != null)
                    {
                        estabelecimentoModel = (new RepositoryService()).Estabelecimento.Retrieve(Estabelecimento.Id);
                    }
                    return estabelecimentoModel;
                }
                set { estabelecimentoModel = value; }
            }

        [LogicalAttribute("itbc_faturamento_parcial")]
            public bool FaturamentoParcial { get; set; }

            [LogicalAttribute("itbc_inscricao_estadual")]
            public String InscricaoEstadual { get; set; }

            [LogicalAttribute("itbc_keyaccountourepresentanteid")]
            public Lookup KeyAccountRepresentante { get; set; }

            [LogicalAttribute("itbc_mensagem")]
            public Lookup Mensagem { get; set; }

            [LogicalAttribute("itbc_modalidade")]
            public int? Modalidade { get; set; }

            [LogicalAttribute("itbc_motivo_bloqueio_credito")]
            public String MotivoBloqueioCredito { get; set; }

            [LogicalAttribute("itbc_motivo_liberacao_credito")]
            public String MotivoLiberacaoCredito { get; set; }

            [LogicalAttribute("itbc_natureza_operacao")]
            public Lookup NaturezaOperacao { get; set; }

            [LogicalAttribute("itbc_nome_abreviado")]
            public String NomeAbreviado { get; set; }

            [LogicalAttribute("itbc_origem")]
            public int? Origem { get; set; }

            [LogicalAttribute("itbc_pedido_cliente")]
            public String PedidoCliente { get; set; }

            [LogicalAttribute("itbc_pedido_ems")]
            public String PedidoEMS { get; set; }

            [LogicalAttribute("itbc_pedido_representante")]
            public String PedidoRepresentante { get; set; }

            [LogicalAttribute("itbc_percentual_desconto_icms")]
            public Decimal? PercentualDescontoICMS { get; set; }

            [LogicalAttribute("itbc_portador")]
            public Lookup Portador { get; set; }

            [LogicalAttribute("itbc_prioridade")]
            public int? Prioridade { get; set; }

            [LogicalAttribute("itbc_rota")]
            public Lookup Rota { get; set; }

            [LogicalAttribute("itbc_salesorder")]
            public Lookup PedidoOriginal { get; set; }

            [LogicalAttribute("itbc_shipto_city")]
            public Lookup EnderecoEntregaCidade { get; set; }

            [LogicalAttribute("itbc_shipto_country")]
            public Lookup EnderecoEntregaPais { get; set; }

            [LogicalAttribute("itbc_shipto_number")]
            public String EnderecoEntregaNumero { get; set; }

            [LogicalAttribute("itbc_shipto_postofficebox")]
            public String EnderecoEntregaCaixaPostal { get; set; }

            [LogicalAttribute("itbc_shipto_stateorprovince")]
            public Lookup EnderecoEntregaEstado { get; set; }

            [LogicalAttribute("itbc_shipto_street")]
            public String EnderecoEntregaRua { get; set; }

            [LogicalAttribute("itbc_situacao_alocacao")]
            public Int32 SituacaoAlocacao { get; set; }

            [LogicalAttribute("itbc_tabela_preco_ems")]
            public String TabelaPrecoEMS { get; set; }

            [LogicalAttribute("itbc_tabeladefinanciamento")]
            public Lookup TabelaFinanciamento { get; set; }

            [LogicalAttribute("itbc_tabeladepreco")]
            public Lookup TabelaPreco { get; set; }

            [LogicalAttribute("itbc_tipo_pedido")]
            public String TipoPedido { get; set; }

            [LogicalAttribute("itbc_tipo_preco")]
            public int? TipoPreco { get; set; }

            [LogicalAttribute("itbc_totalipi")]
            public Decimal? TotalIPI { get; set; }

            [LogicalAttribute("itbc_totalst")]
            public Decimal? TotalSubstituicaoTributaria { get; set; }

            [LogicalAttribute("itbc_transportadora")]
            public Lookup Transportadora { get; set; }

            [LogicalAttribute("itbc_usuario_alteracao")]
            public String UsuarioAlteracao { get; set; }

            [LogicalAttribute("itbc_usuario_aprovacao")]
            public String UsuarioAprovacao { get; set; }

            [LogicalAttribute("itbc_usuario_cancelamento")]
            public String UsuarioCancelamento { get; set; }

            [LogicalAttribute("itbc_usuario_reativacao")]
            public String UsuarioReativacao { get; set; }

            [LogicalAttribute("itbc_usuario_suspensao")]
            public String UsuarioSuspensao { get; set; }

            [LogicalAttribute("itbc_valor_credito_liberado")]
            public Decimal? ValorCreditoLiberado { get; set; }

            [LogicalAttribute("itbc_valor_mercadoria_aberto")]
            public Decimal? ValorMercadoriaAberto { get; set; }

            [LogicalAttribute("itbc_valor_total_aberto")]
            public Decimal? ValorTotalAberto { get; set; }

            [LogicalAttribute("itbc_vlrtotalprodutossemipiest")]
            public Decimal? ValorTotalProdutosSemIPI { get; set; }

            [LogicalAttribute("name")]
            public String Nome { get; set; }

            [LogicalAttribute("opportunityid")]
            public Lookup Oportunidade { get; set; }

            [LogicalAttribute("ordernumber")]
            public String IDPedido { get; set; }

            [LogicalAttribute("paymenttermscode")]
            public int? CondicoesPagamento { get; set; }

            [LogicalAttribute("pricelevelid")]
            public Lookup ListaPreco { get; set; }
        
            [LogicalAttribute("quoteid")]
            public Lookup Cotacao { get; set; }
        
            [LogicalAttribute("requestdeliveryby")]
            public DateTime? DataEntregaSolicitada { get; set; }
            
            [LogicalAttribute("shipto_contactname")]
            public String NomeContatoEntrega { get; set; }

            [LogicalAttribute("shipto_fax")]
            public String FaxEntrega { get; set; }

            [LogicalAttribute("shipto_freighttermscode")]
            public int? CondicoesFreteEntrega { get; set; }

            //[LogicalAttribute("shipto_line1")]
            //public String Rua1Entrega { get; set; }

            [LogicalAttribute("shipto_line2")]
            public String BairroEntrega { get; set; }

            [LogicalAttribute("shipto_line3")]
            public String ComplementoEntrega { get; set; }

            //[LogicalAttribute("shipto_name")]
            //public String NomeEntrega { get; set; }

            [LogicalAttribute("shipto_postalcode")]
            public String CEPEntrega { get; set; }

            //[LogicalAttribute("shipto_stateorprovince")]
            //public String EstadoEntrega { get; set; }

            [LogicalAttribute("shipto_telephone")]
            public String TelefoneEntrega { get; set; }

            //[LogicalAttribute("tipodeobjeto")]
            //public String TipoObjeto { get; set; }
            
            //propriedades que devem ser revisadas - TIPOCUSTOMER E TIPOPROPRIETARIO
            //Na documentação diz que existem apenas na mensagem
            //[LogicalAttribute("tipocustomer")]
            //public String TipoCliente { get; set; }
            
            //[LogicalAttribute("tipoproprietario")]
            //public String TipoProprietario { get; set; }    

            [LogicalAttribute("totalamount")]
            public Decimal? ValorTotal { get; set; }

            //[LogicalAttribute("totalamountlessfreight")]
            //public Decimal? ValorTotalSemFrete { get; set; }

            //[LogicalAttribute("totaldiscountamount")]
            //public Decimal? DescontoTotal { get; set; }

            //[LogicalAttribute("totallineitemamount")]
            //public Decimal? ValorTotalProdutos { get; set; }

            //[LogicalAttribute("totaltax")]
            //public Decimal? TotalImpostos { get; set; }

            [LogicalAttribute("transactioncurrencyid")]
            public Lookup Moeda { get; set; }

            [LogicalAttribute("willcall")]
            public Boolean Remessa { get; set; }

            [LogicalAttribute("itbc_assistente")]
            public Lookup Assistente { get; set; }

            [LogicalAttribute("itbc_supervisor")]
            public Lookup Supervisor { get; set; }
        
            [LogicalAttribute("itbc_solicitacaodebeneficioid")]
            public Lookup SolicitacaoBeneficio { get; set; }

            [LogicalAttribute("itbc_nomeusuariocriacao")]
            public string NomeUsuarioCriacao { get; set; }

            [LogicalAttribute("itbc_tipousuariocriacao")]
            public int? TipoUsuarioCriacao { get; set; }

            [LogicalAttribute("itbc_supervoriginal")]
            public string SupervisorOriginal { get; set; }

            [LogicalAttribute("itbc_assistoriginal")]
            public string AssistenteOriginal { get; set; }

            [LogicalAttribute("itbc_reproriginal")]
            public string RepresentanteOriginal { get; set; }

            [LogicalAttribute("itbc_integradocomerros")]
            public Boolean? IntegradoComErros { get; set; }

            [LogicalAttribute("itbc_errointegrassistente")]
            public Boolean? IntegradoAssistenteComErro { get; set; }

            [LogicalAttribute("itbc_errointegrsuperv")]
            public Boolean? IntegradoSupervisorComErro { get; set; }

            [LogicalAttribute("itbc_errointegrrepresent")]
            public Boolean? IntegradoRepresentanteComErro { get; set; }

            
            private int statusPedido;

            [LogicalAttribute("statuscode")]
            public int StatusPedido
            {
                get {
                    return statusPedido;
                }
                set {
                    statusPedido = value;
                    StatusDoPedido = (StatusDoPedido) StatusDoPedido.Parse(StatusDoPedido.GetType(), statusPedido.ToString());
                }
            }

        
        [LogicalAttribute("itbc_totalst")]
        public decimal SubstituicaoTributaria { get; set; }

        [LogicalAttribute("itbc_condicoesespeciais")]
        public string DescricaoNota { get; set; }

        public TabelaDePreco TabelaDePreco { get; set; }

        public UnidadeNegocio UnidadeDeNegocio { get; set; }

        //[LogicalAttribute("itbc_representante")]
        public Lookup RepresentanteId { get; set; }

        public Contato Representante { get; set; }

        public StatusDoPedido StatusDoPedido { get; set; }

        public bool Vendor { get; set; }    

        [LogicalAttribute("itbc_data_emissao")]
        public DateTime DataDeEmissao { get; set; }

        [LogicalAttribute("itbc_data_entrega")]
        public DateTime DataDeFaturamento { get; set; }

        public DateTime DataDeVencimento { get; set; }

        [LogicalAttribute("itbc_data_negociacao")]
        public DateTime DataBaseNegociacao { get; set; }

        [LogicalAttribute("itbc_pedido_ems")]
        public string CodigoDoPedido { get;  set; }

        [LogicalAttribute("totalamount")]
        public decimal PrecoTotal { get; set; }

        [LogicalAttribute("itbc_vlrtotalprodutossemipiest")]
        public decimal PrecoTotalComIPI { get; set; }

        private CondicaoPagamento condicaoDePagamento = null;
        public CondicaoPagamento CondicaoDePagamento
        {
            get
            {
                if (condicaoDePagamento == null && CondicaoPagamento != null)
                {
                    condicaoDePagamento = (new RepositoryService()).CondicaoPagamento.Retrieve(CondicaoPagamento.Id);
                }
                return condicaoDePagamento;
            }
            set { condicaoDePagamento = value; }
        }

        private Model.Conta cliente = null;
        public Model.Conta Cliente
        {
            get
            {
                if (cliente == null && ClienteID != null)
                {
                    cliente = (new RepositoryService()).Conta.Retrieve(ClienteID.Id);
                }
                return cliente;
            }
            set { cliente = value; }
        }

        private List<ProdutoPedido> itensDoPedido = new List<ProdutoPedido>();
        public List<ProdutoPedido> ItensDoPedido
        {
            get { return itensDoPedido; }
            set { itensDoPedido = value; }
        }

        #endregion
    }
}
