using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Enum;
using System.Threading;
using System.IO;

namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    [LogicalEntity("new_diagnostico_ocorrencia")]
    public class Diagnostico : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Diagnostico() { }

        public Diagnostico(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Diagnostico(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("new_name")]
        public String Nome { get; set; }
        public Enum.StatusDoDiagnostico StatusDiagnostico
        {
            get { return (RazaoStatus.HasValue ? (Enum.StatusDoDiagnostico)RazaoStatus.Value : Enum.StatusDoDiagnostico.Vazio); }
            set { }
        }

        public String DescricaoDaMensagemDeIntegracao { get; set; }
        public String CodigoMatriz { get; set; }
        [LogicalAttribute("new_numerodopedido")]
        public string NumeroDoPedido { get; set; }
        [LogicalAttribute("itbc_logpedido")]
        public string LogPedido { get; set; }
        [LogicalAttribute("new_ocorrenciaid")]
        public Lookup OcorrenciaId { get; set; }
        private Ocorrencia _ocorrencia = null;

        //Não descomentar esse código, CRM entra em looping.
        public Ocorrencia Ocorrencia 
        {
            get
            {
                //if (_ocorrencia == null && this.Id != Guid.Empty)
                //    _ocorrencia = RepositoryService.Ocorrencia.ObterPor(this);

                return _ocorrencia;
            }
            set { _ocorrencia = value; }
            }
        [LogicalAttribute("new_qtd_solicitada")]
        public int? QuantidadeSolicitada { get; set; }
        [LogicalAttribute("new_quantidade_substituida")]
        public decimal? QuantidadeSubstituida { get; set; }
        [LogicalAttribute("new_regra_percentual_minimo")]
        public bool? RegraExcessaoPedido { get; set; }
        [LogicalAttribute("new_rastreamento_objeto")]
        public string NumeroRastreamento { get; set; }
        [LogicalAttribute("new_nota_fiscal")]
        public string NumeroNotaFiscal { get; set; }
        [LogicalAttribute("new_serie_nota_fiscal")]
        public string SerieNotaFiscal { get; set; }
        [LogicalAttribute("new_data_emissao")]
        public DateTime? DataFaturamentoERP { get; set; }
        public DateTime? DataPedidoEmitidoERP { get; set; }
        public bool? PedidoEmitidoERP { get; set; }
        [LogicalAttribute("new_data_geracao_pedido")]
        public DateTime? DataGeracaoPedido { get; set; }
        [LogicalAttribute("new_geratroca")]
        public bool? GeraTroca { get; set; }
        [LogicalAttribute("new_posicao_peca")]
        public string Posicao { get; set; }
        [LogicalAttribute("new_pedidoid")]
        public Lookup PedidoDeVendaId { get; set; }
        private Pedido pedidoDeVenda = null;

        [LogicalAttribute("new_peca_em_estoque")]
        public bool? pecaEmEStoque { get; set; }

        public Pedido PedidoDeVenda
        {
            get { return pedidoDeVenda; }
            set { pedidoDeVenda = value; }
        }
        public decimal? Valor { get; set; }
        [LogicalAttribute("new_produtoid")]
        public Lookup ProdutoId { get; set; }
        private Model.Product _Produto;
        public Model.Product Produto
        {
            get
            {
                if (ProdutoId != null && _Produto == null)
                    _Produto = (new CRM2013.Domain.Servicos.RepositoryService()).Produto.Retrieve(this.ProdutoId.Id);
                return _Produto;
            }
            set
            {
                _Produto = value;
            }
        }

        [LogicalAttribute("new_produto_substitutoid")]
        public Lookup ProdutoSubstitutoId { get; set; }
        private Model.Product produtoSubstituto = null;
        public Model.Product ProdutoSubstituto
        {
            get { return produtoSubstituto; }
            set { produtoSubstituto = value; }
        }
        [LogicalAttribute("new_servicoid")]
        public Lookup SolucaoId { get; set; }
        private Solucao _Solucao = null;
        public Solucao Solucao
        {
            get
            {
                if (SolucaoId != null && _Solucao == null)
                    _Solucao = (new CRM2013.Domain.Servicos.RepositoryService()).Solucao.Retrieve(this.SolucaoId.Id);
                return _Solucao;
            }
            set
            {
                _Solucao = value;
            }
        }
        [LogicalAttribute("new_defeitoid")]
        public Lookup DefeitoId { get; set; }
        private Defeito _Defeito = null;
        public Defeito Defeito
        {
            get
            {
                if (DefeitoId != null && _Defeito == null)
                    _Defeito = (new CRM2013.Domain.Servicos.RepositoryService()).Defeito.Retrieve(this.DefeitoId.Id);
                return _Defeito;
            }
            set
            {
                _Defeito = value;
            }
        }
        [LogicalAttribute("new_valor_base_icms")]
        public Decimal? ValorBaseICMS { get; set; }
        [LogicalAttribute("new_valor_icms")]
        public Decimal? ValorICMS { get; set; }
        [LogicalAttribute("new_valor_ipi")]
        public Decimal? ValorIPI { get; set; }
        [LogicalAttribute("new_aliquota_ipi")]
        public Decimal? AliquotaIPI { get; set; }
        [LogicalAttribute("new_valor_unitario")]
        public Decimal? ValorUnitario { get; set; }
        [LogicalAttribute("new_qtd_faturada")]
        public int? QuantidadeFaturada { get; set; }
        [LogicalAttribute("itbc_estabelecimento")]
        public Lookup EstabelecimentoId { get; set; }
        
        [LogicalAttribute("new_diagnostico_paiid")]
        public Lookup DiagnosticoPaiId { get; set; }

        public Diagnostico _diagnosticoPai;
        public Diagnostico DiagnosticoPai
        {
            get
            {
                if (_diagnosticoPai == null && DiagnosticoPaiId != null)
                    _diagnosticoPai = (new RepositoryService()).Diagnostico.Retrieve(DiagnosticoPaiId.Id);
                return _diagnosticoPai;
            }
            set { _diagnosticoPai = value; }
        }
        
        [LogicalAttribute("new_extrato_logistica_reversaid")]
        public Lookup ExtratoId { get; set; }
        private LogisticaReversa _Extrato = null;
        public LogisticaReversa Extrato
        {
            get
            {
                if (_Extrato == null && ExtratoId != null)
                    _Extrato = (new RepositoryService()).LogisticaReversa.Retrieve(ExtratoId.Id);
                return _Extrato;
            }
            set { _Extrato = value; }
        }

        #endregion

        #region Métodos

        private struct ItemParaArquivo
        {
            public string CodigoPosto { get; set; }
            public string CodigoEstabelecimento { get; set; }
            public string CodigoAtendente { get; set; }
        };

        public decimal EfetuaValidacaoPercoItemAstec()
        {
            if (this.Ocorrencia.ValorServico == decimal.MinValue)
                throw new ArgumentException("Ocorrência " + this.Ocorrencia.Numero + " não contem valor de serviço cadastrado!");

            List<string> mensagemErro;
            decimal preco = RepositoryService.Diagnostico.PrecoParaGerarPedidoAstec(this, out mensagemErro);

            if (mensagemErro.Count > 0)
            {
                string mensagem = string.Format("Número da Ocorrência: {0} <br />Produto: {1} - {2} <br />",
                                         this.Ocorrencia.Numero,
                                         this.Produto.Nome,
                                         this.Produto.Codigo);

                foreach (string item in mensagemErro)
                    mensagem += item + " <br />";

                throw new ArgumentException(mensagem);
            }

            return preco;
        }

        /// <summary>
        /// Verifica a condição que o metodo deve entrar na rotina de pedido
        /// </summary>
        /// <param name="mensagem">Retorna o detalha da condição</param>
        /// <returns></returns>
        public CondicaoPedidoAstec DeveFazerPedidoAstec(out string mensagem)
        {
            mensagem = string.Empty;
            if (this.Ocorrencia == null && this.OcorrenciaId != null)
                this.Ocorrencia = RepositoryService.Ocorrencia.Retrieve(this.OcorrenciaId.Id);
            if (this.Ocorrencia.IntervencaoTecnicaEmAnalise) return CondicaoPedidoAstec.NenhumaAcao;
            if (this.QuantidadeSolicitada <= 0)
            {
                mensagem = "Quantidade solicitada é zero!";
                return CondicaoPedidoAstec.AvancarStatus;
            }

            if (!this.Ocorrencia.EfetuaValidacaoDePedidoAstec())
                return CondicaoPedidoAstec.NenhumaAcao;

            if (this.Ocorrencia.Produto.LinhaComercial.PercentualMinimoParaPedido <= 0)
                return CondicaoPedidoAstec.GerarPedido;

            decimal valor = decimal.MinValue;
            try
            {
                valor = this.EfetuaValidacaoPercoItemAstec();

                if (valor == decimal.MinValue)
                    return CondicaoPedidoAstec.NenhumaAcao;
            }
            catch (Exception ex)
            {
                //this.Ocorrencia.EnviaEmailDeAvisoExportacaoPedidoAstec("Não contem valor de serviço cadastrado!");
                mensagem = ex.Message;
                return CondicaoPedidoAstec.NenhumaAcao;
            }

            var percentualMinimo = this.Ocorrencia.Produto.LinhaComercial.PercentualMinimoParaPedido;

            if (valor > this.Ocorrencia.ValorServico * (Convert.ToDecimal(percentualMinimo) / 100))
            {
                return CondicaoPedidoAstec.GerarPedido;
            }
            else
            {
                mensagem = "Não atingiu a regra de percentual mínimo!"
                         + Environment.NewLine + "Número da Ocorrência: " + this.Ocorrencia.Numero
                         + Environment.NewLine + "Produto: " + this.Produto.Nome + " - " + this.Produto.Codigo
                         + Environment.NewLine + "Valor: " + valor
                         + Environment.NewLine + "Percentual Mínimo: " + percentualMinimo;

                return CondicaoPedidoAstec.AvancarStatus;
            }
        }

        public void Mesclar(Diagnostico diagnostico)
        {
            if (this.Id == Guid.Empty) this.Id = diagnostico.Id;
            if (this.produtoSubstituto == null) this.ProdutoSubstituto = diagnostico.ProdutoSubstituto;
            if (this.EstabelecimentoId != null) this.EstabelecimentoId = diagnostico.EstabelecimentoId;
            if (string.IsNullOrEmpty(this.NumeroNotaFiscal)) this.NumeroNotaFiscal = diagnostico.NumeroNotaFiscal;
            if (string.IsNullOrEmpty(this.SerieNotaFiscal)) this.SerieNotaFiscal = diagnostico.SerieNotaFiscal;
            if (string.IsNullOrEmpty(this.NumeroRastreamento)) this.NumeroRastreamento = diagnostico.NumeroRastreamento;
            if (this.DataFaturamentoERP == DateTime.MinValue) this.DataFaturamentoERP = diagnostico.DataFaturamentoERP;
            if (this.QuantidadeFaturada == int.MinValue) this.QuantidadeFaturada = diagnostico.QuantidadeFaturada;
            if (this.ValorUnitario == decimal.MinValue) this.ValorUnitario = diagnostico.ValorUnitario;
            if (this.AliquotaIPI == decimal.MinValue) this.AliquotaIPI = diagnostico.AliquotaIPI;
            if (this.ValorIPI == decimal.MinValue) this.ValorIPI = diagnostico.ValorIPI;
            if (this.ValorICMS == decimal.MinValue) this.ValorICMS = diagnostico.ValorICMS;
            if (this.ValorBaseICMS == decimal.MinValue) this.ValorBaseICMS = diagnostico.ValorBaseICMS;
        }

        public List<string> MontaArquivosASTEC(List<Diagnostico> servicos)
        {
            List<string> arquivos = new List<string>();
            string mensagem = string.Empty;

            try
            {
                foreach (Diagnostico item in servicos)
                {
                    var incluir = true;
                    string codigoPosto = "", codigoEstabelecimento = "", codigoProduto = "", idProduto = "", numeroOS = "",
                        qtdeSolicitada = "", codigoTransportadora = "", codigoAtendente = "", canalDeVenda = "0       ";

                    try { codigoPosto = (item.Nome + "       ").Substring(0, 6); }
                    catch (Exception ex) { mensagem += string.Format("Posto: [{0}] - {1}.\n", item.Nome, ex.Message); }

                    try { codigoEstabelecimento = (item.Produto.LinhaComercial.Estabelecimento.Codigo.ToString() + "   ").Substring(0, 3); }
                    catch (Exception ex) { mensagem += string.Format("Estabelecimento: {0}.\n", ex.Message); }

                    try { codigoProduto = (item.ExportaERP + "       ").Substring(0, 7); }
                    catch (Exception ex) { mensagem += string.Format("Produto: [{0}] - {1}.\n", item.ExportaERP, ex.Message); }

                    try { idProduto = (item.DescricaoDaMensagemDeIntegracao + "                                    ").Substring(0, 36); }
                    catch (Exception ex) { mensagem += string.Format("Id Produto: [{0}] - {1}.\n", item.DescricaoDaMensagemDeIntegracao, ex.Message); }

                    try { numeroOS = (item.CodigoMatriz + "                    ").Substring(0, 20); }
                    catch (Exception ex) { mensagem += string.Format("Numero OS: [{0}] - {1}.\n", item.CodigoMatriz, ex.Message); }

                    try { qtdeSolicitada = item.QuantidadeSolicitada.Value.ToString("D5"); }
                    catch (Exception ex) { mensagem += string.Format("Qtde: [{0}] - {1}.\n", item.QuantidadeSolicitada, ex.Message); }

                    try { codigoTransportadora = (item.Posicao + "           ").Substring(0, 11); }
                    catch (Exception ex) { mensagem += string.Format("Transportadora: [{0}] - {1}.\n", item.Posicao, ex.Message); }

                    try { codigoAtendente = (item.Produto.LinhaComercial.CodigoAtendente + "  ").Substring(0, 2); }
                    catch (Exception ex) { mensagem += string.Format("Atendente: {0}.\n", ex.Message); }

                    try { canalDeVenda = (item.Produto.LinhaComercial.CanalDeVenda.CodigoVenda + "        ").Substring(0, 8); }
                    catch (Exception ex) { mensagem += string.Format("Canal Venda: {0}.\n", ex.Message); }

                    for (int i = 0; i < arquivos.Count; i++)
                        if (arquivos[i].Substring(1, 6).Trim() == codigoPosto.Trim() && arquivos[i].Substring(38, 3).Trim() == codigoEstabelecimento)
                        {
                            arquivos[i] = arquivos[i] + "%NL%2" + codigoProduto + qtdeSolicitada + numeroOS + idProduto;
                            incluir = false;
                            break;
                        }

                    if (incluir)
                    {
                        //itensParaArquivo.Add(new ItemParaArquivo() { CodigoPosto = codigoPosto.Trim(), CodigoEstabelecimento = codigoEstabelecimento.Trim(), CodigoAtendente = codigoAtendente.Trim() });
                        arquivos.Add("1" + codigoPosto + DateTime.Now.ToString("dd/MM/yy").Replace("/", "") + codigoTransportadora + DateTime.Now.ToString("MM/yy").Replace("/", "") + "          " + codigoEstabelecimento + codigoAtendente + canalDeVenda + "%NL%2" + codigoProduto + qtdeSolicitada + numeroOS + idProduto);
                    }
                }

                foreach (string item in arquivos)
                    try
                    {
                        using (StreamWriter outfile = new StreamWriter("c:\\temp\\PedidosASTEC\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString() + ".ast"))
                        {
                            outfile.Write(item.Replace("%NL%", Environment.NewLine) + Environment.NewLine);
                            //Muitos arquivos não estavam gerando pois estava indo muito rápido a função, por isso aguarda 1 segundo
                            Thread.Sleep(100);
                        }
                    }
                    catch (Exception ex) { mensagem += string.Format("Arquivo Backup: {0}.\n", ex.Message); }
            }
            catch (Exception ex) { mensagem += string.Format("Arquivo: [{0}] - {1}.\n", arquivos.ToString(), ex.Message); }

            //if (!String.IsNullOrEmpty(mensagem)) EventLog.WriteEntry("CRM ASTEC Erro Arquivo", "CRM ASTEC Erros\n" + mensagem);

            return arquivos;
        }

        public Diagnostico this[Guid id, params string[] columns]
        {
            get
            {
                return RepositoryService.Diagnostico.Retrieve(id, columns);
            }
        }

        /// <summary>
        /// O método vai obter o registro da base com o Id da Classe, depois irá validar as informações para a exclusão.
        /// </summary>
        /// <returns></returns>
        public bool PodeCancelar() {

            if (this.Id == Guid.Empty)
                return true;
            
            if(!string.IsNullOrEmpty(this.NumeroNotaFiscal))
                return false;

            if(!this.RazaoStatus.HasValue)
                return false;

            if (this.RazaoStatus.Value == (int)StatusDoDiagnostico.AguardandoPeca)
                return true;

            if (this.RazaoStatus.Value == (int)StatusDoDiagnostico.PedidoSolicitadoAoEms
                || this.RazaoStatus.Value == (int)StatusDoDiagnostico.ConsertoRealizado
                || this.RazaoStatus.Value == (int)StatusDoDiagnostico.Substituido
                || this.RazaoStatus.Value == (int)StatusDoDiagnostico.IntervencaoTecnica
                || this.RazaoStatus.Value == (int)StatusDoDiagnostico.Cancelado)
                return false;


            return true;
        }

        public void Cancelar()
        {
            if (this.Id == Guid.Empty)
                return;

            if (!PodeCancelar())                
                throw new Exception("O diagnóstico não pode ser cancelado/excluído!");

            RepositoryService.Diagnostico.Cancelar(this.Id);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || !(obj is Diagnostico))
                return false;
            else
                return this.Id == ((Diagnostico)obj).Id;
        }

        #endregion
    }
}