using System;
using System.Linq;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.Servicos.GestaoSLA;

namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    [LogicalEntity("incident")]
    public class Ocorrencia : DomainBase
    {
        #region Atributos
        [LogicalAttribute("incidentid")]
        public Guid? ID { get; set; }
        [LogicalAttribute("title")]
        public String Nome { get; set; }
        [LogicalAttribute("new_data_ajuste_posto")]
        public DateTime? DataEnvioAjustePosto { get; set; }
        [LogicalAttribute("new_data_fabricacao_produto")]
        public DateTime? DataFabricacaoProduto { get; set; }
        [LogicalAttribute("new_data_compra_intelbras")]
        public DateTime? DataCompraIntelbras { get; set; }
        [LogicalAttribute("new_nr_pedido")]
        public string NumeroPedidoVenda { get; set; }
        [LogicalAttribute("new_numero_nota_fiscal")]
        public string NumeroNotaFiscal { get; set; }
        [LogicalAttribute("new_data_ajuste_posto")]
        public DateTime? DataAjustePosto { get; set; }
        [LogicalAttribute("new_valor_servico")]
        public decimal? ValorServico { get; set; }
        [LogicalAttribute("new_retirado_por_cpf")]
        public string RetiradoPorCPF { get; set; }
        [LogicalAttribute("new_retirado_por")]
        public string RetiradoPorNome { get; set; }
        [LogicalAttribute("new_pre_os")]
        public bool PreOS { get; set; }
        [LogicalAttribute("new_gera_atividade")]
        public bool ManterAberto { get; set; }
        [LogicalAttribute("new_data_entrega_cliente_real")]
        public DateTime? DataDeEntregaClienteDigitada { get; set; }
        [LogicalAttribute("new_data_conserto_produto_real")]
        public DateTime? DataDeConsertoDigitada { get; set; }
        [LogicalAttribute("new_data_entrega_cliente")]
        public DateTime? DataDeEntregaClienteInformada { get; set; }
        [LogicalAttribute("new_data_hora_final_execuo")]
        public DateTime? DataSaidaTecnico { get; set; }
        [LogicalAttribute("new_data_inicio_execucao")]
        public DateTime? DataInicioTecnico { get; set; }
        [LogicalAttribute("new_data_conserto_produto")]
        public DateTime? DataDeConsertoInformada { get; set; }
        [LogicalAttribute("createdon")]
        public DateTime? DataDeAberturaDigitada { get; set; }
        [LogicalAttribute("itbc_reagendamento_visita")]
        public DateTime? DataDeReagendamentoVisita { get; set; }
        [LogicalAttribute("new_os_rascunho")]
        public bool? Rascunho { get; set; }
        [LogicalAttribute("new_intervencao_tecnica")]
        public bool? EmIntervencaoTecnica { get; set; }
        [LogicalAttribute("new_acessorios_adicionais")]
        public string AcessoriosOpcionais { get; set; }
        [LogicalAttribute("new_aparecia_produto")]
        public string AparenciaDoProduto { get; set; }
        [LogicalAttribute("productserialnumber")]
        public string ProdutosDoCliente { get; set; }
        [LogicalAttribute("new_endereco")]
        public string ApelidoEndereco { get; set; }
        [LogicalAttribute("new_os_cliente")]
        public string OsCliente { get; set; }
        [LogicalAttribute("new_contato_visita")]
        public string ContatoVisita { get; set; }
        [LogicalAttribute("new_solicitante_portal")]
        public string SolicitantePortal { get; set; }
        [LogicalAttribute("caseorigincode")]
        public int? Origem { get; set; }
        [LogicalAttribute("new_kilometragem_percorrida")]
        public string KilometragemPercorrida { get; set; }
        [LogicalAttribute("new_data_origem")]
        public DateTime? DataOrigem { get; set; }
        [LogicalAttribute("new_data_hora_escalacao")]
        public DateTime? DataEscalacao { get; set; }
        [LogicalAttribute("followupby")]
        public DateTime? DataSLA { get; set; }
        [LogicalAttribute("new_guid_endereco")]
        public string EnderecoId { get; set; }
        [LogicalAttribute("new_pode_habilitar_conclusao_os")]
        public bool PodeHabilitarConclusaoOs { get; set; }
        [LogicalAttribute("new_contato_autorizada")]
        public string Contato { get; set; }
        [LogicalAttribute("new_atividade_executada")]
        public string AtividadeExecutada { get; set; }
        [LogicalAttribute("new_data_hora_prevista_visita")]
        public DateTime? DataPrevistaParaVisita { get; set; }
        [LogicalAttribute("description")]
        public string DefeitoAlegado { get; set; }
        [LogicalAttribute("new_os_lote")]
        public bool? OsEmLote { get; set; }
        [LogicalAttribute("new_nr_fatura")]
        public string NumeroNfFatura { get; set; }
        [LogicalAttribute("ticketnumber")]
        public String Numero { get; set; }
        [LogicalAttribute("new_rua")]
        public string Rua { get; set; }
        [LogicalAttribute("new_bairro")]
        public string Bairro { get; set; }
        [LogicalAttribute("new_cidade")]
        public string Cidade { get; set; }
        [LogicalAttribute("new_uf")]
        public string Estado { get; set; }
        [LogicalAttribute("new_codigo_postagem")]
        public string CodigoPostagemCorreios { get; set; }
        [LogicalAttribute("new_numero_objeto")]
        public string NumeroObjetoPostagemCorreios { get; set; }
        [LogicalAttribute("new_descricao_situacao_postagem")]
        public string SituacaoPostagemCorreios { get; set; }
        [LogicalAttribute("new_numero_nf_consumidor")]
        public string NumeroNfConsumido { get; set; }
        [LogicalAttribute("new_cpf_cnpj")]
        public string CpfCnpjNfConsumido { get; set; }
        [LogicalAttribute("itbc_anexo")]
        public string Anexo { get; set; }

        public List<Assunto> EstruturaDeAssunto { get; set; }
        [LogicalAttribute("new_descricao_asstec")]
        public string DescricaoDaMensagemDeIntegracao { get; set; }
        public StatusDaOcorrencia StatusDaOcorrencia
        {
            get { return (RazaoStatus.HasValue ? (StatusDaOcorrencia)RazaoStatus.Value : StatusDaOcorrencia.Vazio); }
            set { }
        }
        private List<Diagnostico> _Diagnosticos;
        public List<Diagnostico> Diagnosticos
        {
            get
            {
                if (_Diagnosticos == null && this.Id != null)
                {
                    _Diagnosticos = new CRM2013.Domain.Servicos.RepositoryService().Diagnostico.ListarPor(this);
                }
                return _Diagnosticos;
            }
            set { _Diagnosticos = value; }
        }

        [LogicalAttribute("new_reincidenteid")]
        public Lookup OcorrenciaPaiId { get; set; }
        public Ocorrencia OcorrenciaPai { get; set; } //Nao preencher para náo entrar em Loop

        [LogicalAttribute("casetypecode")]
        public int? TipoDeOcorrencia { get; set; }
        public string Tipo
        {
            get { return (TipoDeOcorrencia.HasValue ? ((Enum.TipoDeOcorrencia)TipoDeOcorrencia).ToString() : string.Empty); }
            set { }
        }

        [LogicalAttribute("new_data_hora_conclusao")]
        public DateTime? DataDeConclusao { get; set; }
        //[LogicalAttribute("")]
        public bool EmAndamento
        {
            get { return (DataDeConclusao.HasValue ? true : false); }
            set { }
        }

        [LogicalAttribute("customerid")]
        public Lookup ClienteId { get; set; }


        private Contato _ClienteOS = null;
        public Contato ClienteOS
        {
            get
            {
                if (_ClienteOS == null && ClienteId != null)
                    _ClienteOS = (new CRM2013.Domain.Servicos.RepositoryService()).Contato.Retrieve(this.ClienteId.Id);
                return _ClienteOS;
            }
            set { _ClienteOS = value; }
        }

        private Model.Conta _Cliente;
        public Model.Conta Cliente
        {
            get
            {
                if (_Cliente == null && this.ClienteId != null)
                {
                    _Cliente = (new CRM2013.Domain.Servicos.RepositoryService()).Conta.Retrieve(this.ClienteId.Id);
                }

                return _Cliente;
            }
            set { _Cliente = value; }
        }

        [LogicalAttribute("new_empresa_executanteid")]
        public Lookup EmpresaExecutanteId { get; set; }

        private Model.Conta _EmpresaExecutante;
        public Model.Conta EmpresaExecutante
        {
            get
            {
                if (_EmpresaExecutante == null && this.EmpresaExecutanteId != null)
                {
                    _EmpresaExecutante = (new CRM2013.Domain.Servicos.RepositoryService()).Conta.Retrieve(this.EmpresaExecutanteId.Id);
                }
                return _EmpresaExecutante;
            }
            set { _EmpresaExecutante = value; }
        }

        [LogicalAttribute("new_acao_final2")]
        public Lookup AcaoFinal { get; set; }

        [LogicalAttribute("new_autorizadaid")]
        public Lookup AutorizadaId { get; set; }
        private Model.Conta _Autorizada = null;
        public Model.Conta Autorizada
        {
            get
            {
                if (AutorizadaId != null && _Autorizada == null && this.Id != Guid.Empty)
                    _Autorizada = (new CRM2013.Domain.Servicos.RepositoryService()).Conta.Retrieve(this.AutorizadaId.Id);
                return _Autorizada;
            }
            set { _Autorizada = value; }
        }

        [LogicalAttribute("productid")]
        public Lookup ProdutoId { get; set; }
        private Product _Produto;
        public Product Produto
        {
            get
            {
                if (_Produto == null && this.ProdutoId != null)
                {
                    _Produto = (new CRM2013.Domain.Servicos.RepositoryService()).Produto.Retrieve(this.ProdutoId.Id);
                }
                return _Produto;
            }
            set { _Produto = value; }
        }

        [LogicalAttribute("contractid")]
        public Lookup ContratoId { get; set; }

        private Contrato _Contrato;
        public Contrato Contrato
        {
            get
            {
                if (_Contrato == null && this.ContratoId != null)
                {
                    _Contrato = (new CRM2013.Domain.Servicos.RepositoryService()).Contrato.Retrieve(this.ContratoId.Id);
                }
                return _Contrato;
            }
            set { _Contrato = value; }
        }

        [LogicalAttribute("contractdetailid")]
        public Lookup LinhaDeContratoId { get; set; }
        private LinhaDeContrato _LinhaDeContrato;
        public LinhaDeContrato LinhaDeContrato
        {
            get
            {
                if (_LinhaDeContrato == null && LinhaDeContratoId != null)
                {
                    _LinhaDeContrato = (new CRM2013.Domain.Servicos.RepositoryService()).LinhaDoContrato.Retrieve(LinhaDeContratoId.Id);
                }
                return _LinhaDeContrato;
            }
            set { _LinhaDeContrato = value; }
        }

        [LogicalAttribute("new_tecnico_visitaid")]
        public Lookup TecnicoDaVisitaId { get; set; }
        private Contato _TecnicoDaVisita;
        public Contato TecnicoDaVisita
        {
            get
            {
                if (_TecnicoDaVisita == null && TecnicoDaVisitaId != null)
                    _TecnicoDaVisita = (new CRM2013.Domain.Servicos.RepositoryService()).Contato.Retrieve(this.TecnicoDaVisitaId.Id);
                return _TecnicoDaVisita;
            }
            set
            {
                _TecnicoDaVisita = value;
            }
        }

        [LogicalAttribute("new_tecnico_responsavelid")]
        public Lookup TecnicoResponsavelId { get; set; }
        private Contato _TecnicoResponsavel;
        public Contato TecnicoResponsavel
        {
            get
            {
                if (_TecnicoResponsavel == null && TecnicoResponsavelId != null)
                    _TecnicoResponsavel = (new CRM2013.Domain.Servicos.RepositoryService()).Contato.Retrieve(this.TecnicoResponsavelId.Id);
                return _TecnicoResponsavel;
            }
            set
            {
                _TecnicoResponsavel = value;
            }
        }

        [LogicalAttribute("new_solicitanteid")]
        public Lookup SolicitanteId { get; set; }
        private Contato _Solicitante;
        public Contato Solicitante
        {
            get
            {
                if (_Solicitante == null && SolicitanteId != null)
                    _Solicitante = (new CRM2013.Domain.Servicos.RepositoryService()).Contato.Retrieve(this.SolicitanteId.Id);
                return _Solicitante;
            }
            set
            {
                _Solicitante = value;
            }
        }

        [LogicalAttribute("new_localidadeid")]
        public Lookup LocalidadeId { get; set; }
        private Localidade _Localidade;
        public Localidade Localidade
        {
            get
            {
                if (_Localidade == null && LocalidadeId != null)
                    _Localidade = (new CRM2013.Domain.Servicos.RepositoryService()).Localidade.Retrieve(this.LocalidadeId.Id);
                return _Localidade;
            }
            set
            {
                _Localidade = value;
            }
        }

        [LogicalAttribute("prioritycode")]
        public int? PrioridadeValue { get; set; }
        public TipoDePrioridade? Prioridade
        {
            get { return (PrioridadeValue.HasValue ? (TipoDePrioridade)PrioridadeValue.Value : TipoDePrioridade.Vazio); }
            set { }
        }

        [LogicalAttribute("new_tipo_coleta_postagem")]
        public int? TipoDeETicketPostagemCorreiosValue { get; set; }
        public TipoDeETicket? TipoDeETicketPostagemCorreios
        {
            get { return (TipoDeETicketPostagemCorreiosValue.HasValue ? (TipoDeETicket)TipoDeETicketPostagemCorreiosValue.Value : TipoDeETicket.Vazio); }
            set { }
        }

        [LogicalAttribute("new_extrato_pagamentoid")]
        public Lookup ReferenciaExtratoPagamentoId { get; set; }
        private Extrato _ReferenciaExtratoPagamento;
        public Extrato ReferenciaExtratoPagamento
        {
            get
            {
                if (_ReferenciaExtratoPagamento == null && ReferenciaExtratoPagamentoId != null)
                    _ReferenciaExtratoPagamento = (new CRM2013.Domain.Servicos.RepositoryService()).Extrato.Retrieve(this.ReferenciaExtratoPagamentoId.Id);
                return _ReferenciaExtratoPagamento;
            }
            set
            {
                _ReferenciaExtratoPagamento = value;
            }
        }

        [LogicalAttribute("new_acaoid")]
        public Lookup AcaoId { get; set; }
        public Acao Acao
        {
            get { return (AcaoId != null ? new Acao() { Id = AcaoId.Id, Nome = AcaoId.Name } : null); }
            set { }
        }

        [LogicalAttribute("subjectid")]
        public Lookup AssuntoId { get; set; }
        public Assunto Assunto
        {
            get { return (AssuntoId != null ? new Assunto() { Id = AssuntoId.Id, Nome = AssuntoId.Name } : null); }
            set { }
        }

        [LogicalAttribute("new_unidade_negocio_astec")]
        public Lookup UnidadeDeNegocioAstecId { get; set; }

        [LogicalAttribute("new_assunto_unidadeid")]
        public Lookup AssuntoUnidadeId { get; set; }
        public Assunto AssuntoUnidade
        {
            get { return (AssuntoUnidadeId != null ? new Assunto() { Id = AssuntoUnidadeId.Id, Nome = AssuntoUnidadeId.Name } : null); }
            set { }
        }

        [LogicalAttribute("new_assunto_tipoid")]
        public Lookup AssuntoTipoId { get; set; }
        public Assunto AssuntoTipo
        {
            get { return (AssuntoTipoId != null ? new Assunto() { Id = AssuntoTipoId.Id, Nome = AssuntoTipoId.Name } : null); }
            set { }
        }

        [LogicalAttribute("new_assunto_serieid")]
        public Lookup AssuntoSerieId { get; set; }
        public Assunto AssuntoSerie
        {
            get { return (AssuntoSerieId != null ? new Assunto() { Id = AssuntoSerieId.Id, Nome = AssuntoSerieId.Name } : null); }
            set { }
        }

        [LogicalAttribute("new_assunto_produtoid")]
        public Lookup AssuntoProdutoId { get; set; }
        public Assunto AssuntoProduto
        {
            get { return (AssuntoProdutoId != null ? new Assunto() { Id = AssuntoProdutoId.Id, Nome = AssuntoProdutoId.Name } : null); }
            set { }
        }

        [LogicalAttribute("new_assunto_problemaid")]
        public Lookup AssuntoProblemaId { get; set; }
        public Assunto AssuntoProblema
        {
            get { return (AssuntoProblemaId != null ? new Assunto() { Id = AssuntoProblemaId.Id, Nome = AssuntoProblemaId.Name } : null); }
            set { }
        }

        [LogicalAttribute("new_assunto_parteid")]
        public Lookup AssuntoParteId { get; set; }
        public Assunto AssuntoParte
        {
            get { return (AssuntoParteId != null ? new Assunto() { Id = AssuntoParteId.Id, Nome = AssuntoParteId.Name } : null); }
            set { }
        }

        [LogicalAttribute("new_assunto_motivoid")]
        public Lookup AssuntoMotivoId { get; set; }
        public Assunto AssuntoMotivo
        {
            get { return (AssuntoMotivoId != null ? new Assunto() { Id = AssuntoMotivoId.Id, Nome = AssuntoMotivoId.Name } : null); }
            set { }
        }

        [LogicalAttribute("new_solucao_asstec")]
        public int? AcaoAssistenciaValue { get; set; }
        public AcaoAssistencia? new_solucao_asstec
        {
            get { return (AcaoAssistenciaValue.HasValue ? (AcaoAssistencia)AcaoAssistenciaValue.Value : AcaoAssistencia.Vazio); }
            set { }
        }

        public List<DefeitoOcorrenciaCliente> DefeitosAlegados
        {
            get { return (this.Id == Guid.Empty ? null : (new CRM2013.Domain.Servicos.RepositoryService()).DefeitoOcorrenciaCliente.ListarPor(this)); }
            set { }
        }

        public List<Anotacao> Anexos
        {
            get { return (this.Id == Guid.Empty ? null : (new CRM2013.Domain.Servicos.RepositoryService()).Anexo.ListarPor(this.Id, false)); }
            set { }
        }
        public List<Auditoria> Auditorias
        {
            get { return (this.Id == Guid.Empty ? null : (new CRM2013.Domain.Servicos.RepositoryService()).Auditoria.ListarPor(this)); }
            set { }
        }

        public IntervencaoTecnica IntervencaoTecnica
        {
            get { return (this.Id == Guid.Empty ? null : (new CRM2013.Domain.Servicos.RepositoryService()).Intervencao.ObterPor(this)); }
            set { }
        }
        public bool IntervencaoTecnicaEmAnalise
        {
            get { return (this.Id == Guid.Empty ? false : ((new CRM2013.Domain.Servicos.RepositoryService()).Intervencao.ListarPor(this, 1 /*em analise*/).Count > 0)); }
            set { }
        }

        //Dados da Nota Fiscal do Consumidor da ocorrencia
        [LogicalAttribute("new_numero_nf_consumidor")]
        public string NumeroNotaFiscalDeCompra { get; set; }
        [LogicalAttribute("new_data_compra")]
        public DateTime? DataConstadoNotaFiscalDeCompra { get; set; }
        [LogicalAttribute("new_nome_nf")]
        public string NomeConstadoNaNotaFiscalDeCompra { get; set; }
        [LogicalAttribute("new_cpf_cnpj")]
        public string CpfCnpjConstadoNaNotaFiscalDeCompra { get; set; }
        [LogicalAttribute("new_nome_loja")]
        public string NomeDaLojaDoAtendimento { get; set; }
        [LogicalAttribute("new_cnpj_loja")]
        public string CnpjDaLojaDoAtendimento { get; set; }
        [LogicalAttribute("new_telefone_loja")]
        public string TelefoneDaLojaDoAtendimento { get; set; }
        private Fatura _NotaFiscalFatura = null;
        public Fatura NotaFiscalFatura
        {
            get
            {
                if (_NotaFiscalFatura == null && this.Id != Guid.Empty)
                {
                    _NotaFiscalFatura = new Fatura();
                    if (this.Origem != 200004 && this.Origem != 200006 && this.PreOS == false)
                        _NotaFiscalFatura = (new CRM2013.Domain.Servicos.RepositoryService()).Fatura.PesquisarNotaFiscalFaturaDaOcorrenciaPor(this.Contrato, this.Cliente);
                    else
                    {
                        _NotaFiscalFatura.IDFatura = NumeroNotaFiscalDeCompra;
                        if (DataConstadoNotaFiscalDeCompra.HasValue)
                            _NotaFiscalFatura.DataEmissao = DataConstadoNotaFiscalDeCompra.Value;
                        _NotaFiscalFatura.Cliente = new Domain.Model.Conta();
                        _NotaFiscalFatura.Cliente.Nome = NomeDaLojaDoAtendimento;
                        _NotaFiscalFatura.Cliente.CpfCnpj = CnpjDaLojaDoAtendimento;
                        _NotaFiscalFatura.Cliente.Telefone = TelefoneDaLojaDoAtendimento;
                        _NotaFiscalFatura.Cliente.NomeAbreviado = NomeConstadoNaNotaFiscalDeCompra;
                        _NotaFiscalFatura.Cliente.DocIdentidade = CnpjDaLojaDoAtendimento;
                    }
                }
                return _NotaFiscalFatura;
            }
            set { _NotaFiscalFatura = value; }
        }
        //Fim dos dados na Nota Fiscal do Consumidor

        [LogicalAttribute("itbc_data_hora_solucao_cliente")]
        public DateTime? DataSolucaoCliente { get; set; }

        [LogicalAttribute("itbc_observacao_orcamento")]
        public string ObsevacaoOrcamento { get; set; }

        [LogicalAttribute("itbc_limite_orcamento")]
        public decimal? LimiteOrcamento { get; set; }

        [LogicalAttribute("itbc_marcaid")]
        public Lookup MarcaId { get; set; }
        private Marca _Marca;
        public Marca Marca
        {
            get
            {
                if (_Marca == null && this.MarcaId != null)
                {
                    _Marca = (new CRM2013.Domain.Servicos.RepositoryService()).Marca.Retrieve(this.MarcaId.Id);
                }
                return _Marca;
            }
            set { _Marca = value; }
        }

        [LogicalAttribute("itbc_modeloid")]
        public Lookup ModeloId { get; set; }
        private Modelo _Modelo;
        public Modelo Modelo
        {
            get
            {
                if (_Modelo == null && this.ModeloId != null)
                {
                    _Modelo = (new CRM2013.Domain.Servicos.RepositoryService()).Modelo.Retrieve(this.ModeloId.Id);
                }
                return _Modelo;
            }
            set { _Modelo = value; }
        }

        [LogicalAttribute("itbc_protocolo_telefonico")]
        public String ProtocoloTelefonico { get; set; }

        [LogicalAttribute("itbc_duracao_chamada")]
        public int? DuracaoChamada { get; set; }

        [LogicalAttribute("itbc_nota_pesquisa")]
        public int? NotaPesquisa { get; set; }

        [LogicalAttribute("itbc_tempo_fila")]
        public int? TempoFila { get; set; }

        [LogicalAttribute("itbc_tempo_wrap")]
        public int? TempoWrap { get; set; }

        [LogicalAttribute("itbc_numero_fila_entrada2")]
        public String NumeroFilaEntrada { get; set; }

        [LogicalAttribute("itbc_codigo_produto_ura")]
        public String CodigoProdutoURA { get; set; }

        [LogicalAttribute("itbc_resumo_ocorrencia")]
        public string ResumoDaOcorrencia { get; set; }

        [LogicalAttribute("new_causa_finalid")]
        public Lookup CausaFinal { get; set; }

        [LogicalAttribute("itbc_integrar_astec")]
        public int? IntegraAstec { get; set; }

        //Atendimento via Chat
        [LogicalAttribute("new_protocolo_chat")]
        public string ProtocoloChat { get; set; }
        [LogicalAttribute("itbc_origem_chat")]
        public string OrigemChat { get; set; }
        [LogicalAttribute("itbc_data_entrada_chat")]
        public DateTime? DataEntradaChat { get; set; }
        [LogicalAttribute("itbc_data_inicio_atendimento")]
        public DateTime? DataInicioAtendimento { get; set; }
        [LogicalAttribute("itbc_data_final_atendimento")]
        public DateTime? DataFinalAtendimento { get; set; }
        [LogicalAttribute("itbc_status_atendimento_chat")]
        public string StatusAtendimentoChat { get; set; }
        [LogicalAttribute("itbc_tempo_fila_chat")]
        public int? TempoNaFilaChat { get; set; }
        [LogicalAttribute("itbc_tempo_atendimento_ativo")]
        public int? TempoAtendimentoAtivo { get; set; }
        [LogicalAttribute("itbc_duracao_atendimento")]
        public int? DuracaoAtendimento { get; set; }
        [LogicalAttribute("itbc_formulario_atendimento")]
        public string FormularioAtendimento { get; set; }
        [LogicalAttribute("statuscode")]
        public int? StatusCode { get; set; }

        [LogicalAttribute("itbc_empresas_atendimento_rejeitado")]
        public string EmpresaAtendimentoRejeitado { get; set; }
        
        //Fim dos dados do atendimento via chat 
        [LogicalAttribute("itbc_veiculo")]
        public Lookup VeiculoId { get; set; }
        private Veiculo _Veiculo;
        public Veiculo Veiculo
        {
            get
            {
                if (_Veiculo == null && this.VeiculoId != null)
                {
                    _Veiculo = (new CRM2013.Domain.Servicos.RepositoryService()).Veiculo.Retrieve(this.VeiculoId.Id);
                }
                return _Veiculo;
            }
            set { _Veiculo = value; }
        }

        [LogicalAttribute("itbc_atualizar_operacoes_suporte")]
        public Boolean? AtualizarOperacoesSuporte { get; set; }

        #endregion

        #region Contrutores
        private RepositoryService RepositoryService { get; set; }

        public Ocorrencia()
        {
        }

        public Ocorrencia(string organization, bool isOffline) : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Ocorrencia(string organization, bool isOffline, object provider) : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Metodos

        public Guid Salvar(Ocorrencia ocorrencia, Contrato contrato, Guid linhaDeContratoId)
        {
            // NOVAS INFORMACOES
            List<LinhaDeContrato> linhasDeContrato = (new CRM2013.Domain.Servicos.RepositoryService()).LinhaDoContrato.ListarPor(contrato);
            var linha = (from l in linhasDeContrato
                         where l.Id == linhaDeContratoId
                         select l).FirstOrDefault();

            if (linha != null)
            {
                if (linha.TipoDeOcorrencia.HasValue)
                    ocorrencia.TipoDeOcorrencia = (int)linha.TipoDeOcorrencia.Value;

                ocorrencia.LinhaDeContrato = linha;
            }

            return (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Ocorrencia.Create(ocorrencia);
        }

        public void Atualizar()
        {
            (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Ocorrencia.Update(this);
        }

        public Guid SalvarOS(Ocorrencia ocorrencia)
        {
            Guid IdOS = Guid.Empty;
            try
            {
                #region Cadastro de Contato no CRM

                if (ocorrencia.ClienteOS != null && ocorrencia.ClienteOS.Id == Guid.Empty && ocorrencia.ClienteOS.CpfCnpj.Length > 14)
                {
                    var cliente = (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Conta.ObterPor(ocorrencia.ClienteOS.CpfCnpj);
                    if (cliente != null)
                    {
                        ocorrencia.Cliente = cliente;
                        ocorrencia.ClienteOS = null;
                        ocorrencia.ClienteId = new Lookup() { Id = cliente.Id, Type = "account" };
                    }
                }

                //se já existir um Cliente, ele nula o ClienteOS
                if (ocorrencia.ClienteOS != null)
                {
                    #region Obtendo ID de Cidade e Estado
                    Municipio cidade = (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Municipio.ObterPorCep(ocorrencia.ClienteOS.Endereco1CEP);
                    if (cidade == null) cidade = (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Municipio.ObterPor(ocorrencia.ClienteOS.Endereco1Estado, ocorrencia.ClienteOS.Endereco1Municipio);

                    if (cidade != null)
                    {
                        ocorrencia.ClienteOS.Endereco1Municipio = cidade.Nome;
                        ocorrencia.ClienteOS.Endereco1Estado = cidade.Estadoid.Name;
                        ocorrencia.ClienteOS.Endereco1Municipioid = new Lookup() { Id = cidade.Id, Type = "itbc_municipios" };
                        ocorrencia.ClienteOS.Endereco1Estadoid = cidade.Estadoid;
                        ocorrencia.ClienteOS.Endereco1Pais = new Lookup() { Id = (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Pais.PesquisarPaisPor(cidade.Estadoid.Id).Id, Type = "itbc_pais" };
                    }
                    #endregion

                    //ocorrencia.ClienteOS.Escolaridade = (int)Domain.Enum.Contato.Escolaridade.Primeiro_Grau_Incompleto;

                    Contato contatoTemp = null;
                    if (!string.IsNullOrEmpty(ocorrencia.ClienteOS.CpfCnpj))
                    {
                        contatoTemp = (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Contato.ObterPor(ocorrencia.ClienteOS.CpfCnpj, "");
                        if (contatoTemp != null)
                            ocorrencia.ClienteOS.Id = contatoTemp.Id;

                    }
                    ocorrencia.ClienteOS.IntegrarNoPlugin = true;

                    if (ocorrencia.ClienteOS.Id == Guid.Empty)
                        ocorrencia.ClienteOS.Id = (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Contato.Create(ocorrencia.ClienteOS);
                    else
                    {
                        ocorrencia.ClienteOS.Mesclar(contatoTemp);
                        (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Contato.Update(ocorrencia.ClienteOS);
                    }
                    ocorrencia.ClienteId = new Lookup() { Id = ocorrencia.ClienteOS.Id, Type = "contact" };
                }

                #endregion

                if (ocorrencia.Id == Guid.Empty)
                    IdOS = (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Ocorrencia.Create(ocorrencia);
                else
                {
                    IdOS = ocorrencia.Id;
                    (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Ocorrencia.Update(ocorrencia);
                }

                if (IdOS != Guid.Empty)
                {
                    #region Salva os Diagnósticos da OS

                    if (ocorrencia.Diagnosticos != null && ocorrencia.Diagnosticos.Count > 0)
                        foreach (var item in ocorrencia.Diagnosticos)
                        {
                            item.Ocorrencia = new Ocorrencia() { Id = IdOS };
                            if (item.Id == Guid.Empty)
                                (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Diagnostico.Create(item);
                            else
                                (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Diagnostico.Update(item);
                        }

                    #endregion

                    if (ocorrencia.AtividadeExecutada != "SalvarFechamento")
                    {
                        #region Salva os Defeitos Alegados da OS
                        (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).DefeitoOcorrenciaCliente.ExcluirPor(ocorrencia);

                        if (ocorrencia.DefeitosAlegados != null && ocorrencia.DefeitosAlegados.Count > 0)
                            foreach (var item in ocorrencia.DefeitosAlegados)
                            {
                                var beDefeitos = new DefeitoOcorrenciaCliente(OrganizationName, IsOffline);
                                beDefeitos.OcorrenciaId = new Lookup(IdOS, "incident");
                                beDefeitos.Nome = item.Nome;
                                beDefeitos.DefeitoId = new Lookup(item.Id, "new_defeito");
                                try
                                {
                                    (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).DefeitoOcorrenciaCliente.Create(beDefeitos);
                                }
                                catch { }
                            }

                        #endregion
                    }


                    if (!ocorrencia.Rascunho.HasValue || !ocorrencia.Rascunho.Value) //Se já salvou os diagnósticos e não vai mais alterar
                        if (ocorrencia.RazaoStatus != null && ocorrencia.RazaoStatus.Value > 0)
                            //Thread.Sleep(5000); //Devido a um Workflow deve esperar algum tempo para nao cadastrar o LOG 2 vezes
                            (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Ocorrencia.Update(ocorrencia); //salva o status da OS

                }
            }
            catch (Exception ex)
            {
                //SDKore.Helper.Log.Logar(ex.Message + ex.StackTrace);
                //LogService.GravaLog(ex, TipoDeLog.PortalAssistenciaTecnica);
                //throw new ArgumentException("Operação não realizada. Foi identificada ocorrência redundante. Verifique 'Posto de Serviço', 'Produto', 'Cliente/Cliente OS', 'Número de Série', 'Número da Nota Fiscal'." + ex.Message, ex);
                throw ex;
            }

            return IdOS;
        }

        private void AtualizarDefeitos()
        {
            (new CRM2013.Domain.Servicos.RepositoryService()).DefeitoOcorrenciaCliente.ExcluirPor(this);
            if (this.DefeitosAlegados != null && this.DefeitosAlegados.Count > 0)
                foreach (var item in this.DefeitosAlegados)
                {
                    DefeitoOcorrenciaCliente defeito = new DefeitoOcorrenciaCliente();
                    defeito.Id = item.Id;
                    defeito.Nome = item.Nome;
                    defeito.OcorrenciaId = new Lookup(this.Id, "incident");
                    (new CRM2013.Domain.Servicos.RepositoryService()).DefeitoOcorrenciaCliente.Create(defeito);
                }
        }

        private void AtualizarAuditorias()
        {
            bool aindaTemAuditoriaAberta = false;
            bool atualizarOcorrencia = false;

            if (this.Auditorias != null)
                foreach (var item in this.Auditorias)
                    //Verifica se todas as auditorias foram respondidas
                    if (!string.IsNullOrEmpty(item.Justificativa) && item.DataFinalizacao.HasValue)
                    {
                        atualizarOcorrencia = true;
                        Auditoria auditoria = new Auditoria();

                        auditoria.Id = item.Id;
                        auditoria.RazaoStatus = 1; // Respondido Posto de Serviço
                        auditoria.DataFinalizacao = DateTime.Now;

                        (new CRM2013.Domain.Servicos.RepositoryService()).Auditoria.Update(auditoria);
                    }
                    else if (string.IsNullOrEmpty(item.Justificativa) && item.DataFinalizacao != DateTime.MinValue)
                        aindaTemAuditoriaAberta = true;

            if (atualizarOcorrencia)
            {
                this.DataAjustePosto = DateTime.Now;
                (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.Update(this);
            }

            if (!aindaTemAuditoriaAberta && this.RazaoStatus.Value == (int)StatusDaOcorrencia.Ajuste_Posto_de_Serviço)
                this.RazaoStatus = (int)StatusDaOcorrencia.Auditoria;
        }

        private void AtualizarIntervencoesTecnicas(List<IntervencaoTecnica> colecaoIntervencoes, int contaQtdeItensQueGeramTroca, bool incluiuNovosDiagnosticos)
        {
            var inclui = true;

            if (this.Produto != null
                && this.Produto.DadosFamiliaComercial != null
                && this.Produto.LinhaComercial != null)
            {
                // Por Dias de reicidencia
                if (this.Produto.LinhaComercial.NumeroDeDiasParaReincidencia > 0 && (this.RazaoStatus.Value == (int)StatusDaOcorrencia.Aguardando_Analise || this.RazaoStatus.Value == (int)StatusDaOcorrencia.Aguardando_Peça))
                {
                    //var ocorrencias = (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.ListarPorDiasDeReicidencia(this);

                    foreach (IntervencaoTecnica interverncao in colecaoIntervencoes)
                        if (interverncao.Nome.Contains("Os reincidente em menos de"))
                        {
                            inclui = false;
                            break;
                        }

                    if (inclui)
                    {
                        this.EmIntervencaoTecnica = true;

                        IntervencaoTecnica intervencao = new IntervencaoTecnica();
                        intervencao.OcorrenciaId = new Lookup(this.Id, "incident");
                        intervencao.Nome = "Os reincidente em menos de " + this.Produto.LinhaComercial.NumeroDeDiasParaReincidencia.ToString() + " dias";
                        intervencao.RazaoStatus = 1;

                        (new CRM2013.Domain.Servicos.RepositoryService()).Intervencao.Create(intervencao);
                    }
                }

                // Por itens nos diagnósticos
                inclui = true;
                if (incluiuNovosDiagnosticos && (this.RazaoStatus.Value == (int)StatusDaOcorrencia.Aguardando_Analise || this.RazaoStatus.Value == (int)StatusDaOcorrencia.Aguardando_Peça))
                    if (this.Produto.LinhaComercial.NumeroDeItensParaReincidencia > 0)
                        if (contaQtdeItensQueGeramTroca >= this.Produto.LinhaComercial.NumeroDeItensParaReincidencia)
                        {
                            //Se já existir uma Intervenção com esse motivo não adiciona
                            foreach (var intervencao in colecaoIntervencoes)
                                if (intervencao.Nome.Contains("Quantidade de itens") && intervencao.RazaoStatus.Value == 1) //se já existir um item que ainda não foi liberado não inclui de novo
                                {
                                    inclui = false;
                                    break;
                                }

                            if (inclui)
                            {
                                this.EmIntervencaoTecnica = true;

                                IntervencaoTecnica intervencao = new IntervencaoTecnica();
                                intervencao.OcorrenciaId = new Lookup(this.Id, "incident");
                                intervencao.Nome = "Quantidade de itens em diagnóstico superior a " + this.Produto.LinhaComercial.NumeroDeItensParaReincidencia.ToString() + " unidades";
                                intervencao.RazaoStatus = 1;

                                (new CRM2013.Domain.Servicos.RepositoryService()).Intervencao.Create(intervencao);
                            }
                        }
            }

            //Produto em intervenção técnica
            inclui = true;
            if (this.Produto != null && this.Produto.IntervencaoTecnica != null && this.Produto.IntervencaoTecnica.Value && (this.RazaoStatus.Value == (int)StatusDaOcorrencia.Aguardando_Analise || this.RazaoStatus.Value == (int)StatusDaOcorrencia.Aguardando_Peça))
            {
                //Se existe uma intervenção em análise ou se já existiu, não inclui;
                foreach (var intervencao in colecaoIntervencoes)
                    if (intervencao.Nome.Contains("Produto da OS em intervenção técnica"))
                        inclui = false;

                if (inclui)
                {
                    this.EmIntervencaoTecnica = true;

                    IntervencaoTecnica intervencao = new IntervencaoTecnica();
                    intervencao.OcorrenciaId = new Lookup(this.Id, "incident");
                    intervencao.Nome = "Produto da OS em intervenção técnica";
                    intervencao.RazaoStatus = 1;

                    (new CRM2013.Domain.Servicos.RepositoryService()).Intervencao.Create(intervencao);
                }
            }
        }

        public void Atualizar(Ocorrencia ocorrencia, Contrato contrato, Guid linhaDeContratoId)
        {
            // NOVAS INFORMACOES
            var linhasDeContrato = (new CRM2013.Domain.Servicos.RepositoryService()).LinhaDoContrato.ListarPor(contrato);
            var linha = (from l in linhasDeContrato
                         where l.Id == linhaDeContratoId
                         select l).FirstOrDefault();

            if (linha != null)
            {
                //if (linha.TipoDeOcorrencia != null)
                ocorrencia.TipoDeOcorrencia = (int)linha.TipoDeOcorrencia;

                ocorrencia.LinhaDeContrato = linha;
            }

            (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.Update(ocorrencia);
        }

        public Ocorrencia ObterOcorrenciaPor(string numeroDaOcorrencia)
        {
            var ocorrencia = (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.ObterPor(numeroDaOcorrencia);

            return ocorrencia;
        }

        public HistoricoDePostagem PesquisarHistoricoOcorrencia(Ocorrencia ocorrencia, string numeroObjeto, string tipoEvento, string statusEvento, DateTime DataHora)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).HistoricoDePostagem.PesquisarHistoricoOcorrencia(ocorrencia, numeroObjeto, tipoEvento, statusEvento, DataHora);
        }

        public SLA CalcularDataDeAtendimento(SLA sla)
        {

            #region Validações
            if (null == sla || null == this.Contrato || null == this.Contrato.Calendario || sla.Tempo == 0 || sla.TempoDeEscalacao == 0)
            {
                return sla;
            }
            #endregion

            #region Verifica Tempo Adicional
            int tempoAdicional = 0;
            int minutosAdicionais = 0;
            decimal multiplicadorDistancia = 0;
            Domain.Model.ClienteParticipanteEndereco endereco = (new Domain.Servicos.RepositoryService(this.OrganizationName, this.IsOffline)).ClienteParticipanteDoEndereco.Retrieve(new Guid(this.EnderecoId));

            if(endereco.Pavimentada.HasValue && (bool)endereco.Pavimentada && this.Contrato.AdicionalPavimentada.HasValue)
            {
                minutosAdicionais = (int)this.Contrato.AdicionalPavimentada;
            }
            else if(this.Contrato.AdicionalNaoPavimentada.HasValue)
            {
                minutosAdicionais = (int)this.Contrato.AdicionalNaoPavimentada;
            }

            if(endereco.DistanciaCapital > 0 && minutosAdicionais > 0 && endereco.DistanciaCapital.HasValue && this.Contrato.AcrescerHoraKm.HasValue)
            {
                multiplicadorDistancia = (decimal)endereco.DistanciaCapital / (int)this.Contrato.AcrescerHoraKm;
                tempoAdicional = (int)(minutosAdicionais * multiplicadorDistancia);
            }
            #endregion

            #region Inicializa Variaveis

            var calendario = new CalendarioDeFeriados();
            var feriados = calendario.ObterFeriadosPor(this.Estado, this.Cidade);
            int SLAemMinutos = sla.Tempo;
            if(sla.TempoSolucao > 0)
            {
                SLAemMinutos = sla.TempoSolucao * 60;
            }
            SLAemMinutos += tempoAdicional;

            int SLAEscalaemMinutos = sla.TempoDeEscalacao * 60;

            #endregion

            sla.DataSLA = CalcularDataDeAtendimento(this.DataOrigem.Value.ToLocalTime(), SLAemMinutos, this.Contrato.Calendario, feriados);

            sla.DataEscalacao = CalcularDataDeAtendimento(this.DataOrigem.Value.ToLocalTime(), SLAEscalaemMinutos, this.Contrato.Calendario, feriados);

            return sla;
        }

        /// <summary>
        /// Autor: Clausio Elmano de Oliveira
        /// Data: 21/12/2010
        /// Descrição: calcula a diferença de horas entre duas datas de uma SLA
        /// </summary>
        /// <param name="sla"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <returns>Qtde de minutos entre duas SLAs</returns>
        public int CalcularDiferencaMinutosSLAEntreDatas(DateTime dataInicial, DateTime dataFinal, CalendarioDeTrabalho calendario, List<Feriado> feriados)
        {
            int difMinutos = 0;

            DateTime dataAtual = dataInicial;
            var dateTime = new DateTime();

            var diaDaSemana = calendario.ObterDiaDaSemana(dataAtual);
            var diaDaSemanaFinal = calendario.ObterDiaDaSemana(dataFinal);

            // Verifica se as Datas inicial e final são Feriados
            Boolean datainicialehFeriado = false;
            Boolean datafinalehFeriado = false;

            foreach (Feriado feriado in feriados)
            {
                if (feriado.DataInicio.Date.ToString("dd/MM/yyyy") == dataAtual.ToString("dd/MM/yyyy"))
                    datainicialehFeriado = true;
                if (feriado.DataInicio.Date.ToString("dd/MM/yyyy") == dataFinal.ToString("dd/MM/yyyy"))
                    datafinalehFeriado = true;
            }

            if (datafinalehFeriado || (diaDaSemana.Inicio.Hours > dataFinal.Hour) || diaDaSemanaFinal.TempoTotalDaJornadaDeTrabalho.ToString() == "00:00:00")
            {
                int idias = -1;
                var dataComp = dataFinal;

                while (dataFinal <= dataComp)
                {
                    dataComp = dateTime.ObterProximoDiaUtil(dataComp.AddDays(idias).Date, calendario, feriados);
                    diaDaSemana = calendario.ObterDiaDaSemana(dataComp);
                    dataComp = new DateTime(dataComp.Year, dataComp.Month, dataComp.Day, diaDaSemana.Fim.Hours, diaDaSemana.Fim.Minutes, diaDaSemana.Fim.Seconds);
                    idias--;
                }

                dataFinal = dataComp;
            }



            diaDaSemana = calendario.ObterDiaDaSemana(dataAtual);

            if (datainicialehFeriado || (diaDaSemana.Fim.Hours <= dataAtual.Hour))
            {
                dataAtual = dateTime.ObterProximoDiaUtil(dataAtual.AddDays(1).Date, calendario, feriados);
                diaDaSemana = calendario.ObterDiaDaSemana(dataAtual);
                dataAtual = new DateTime(dataAtual.Year, dataAtual.Month, dataAtual.Day, diaDaSemana.Inicio.Hours, diaDaSemana.Inicio.Minutes, diaDaSemana.Inicio.Seconds);
            }

            if (diaDaSemana.Inicio.Hours > dataAtual.Hour)
                dataAtual = new DateTime(dataAtual.Year, dataAtual.Month, dataAtual.Day, diaDaSemana.Inicio.Hours, diaDaSemana.Inicio.Minutes, diaDaSemana.Inicio.Seconds);


            while (dataFinal.ToString("dd/MM/yyyy") != dataAtual.ToString("dd/MM/yyyy"))//(dataFinal.Subtract(dataAtual).Days > 0) 
            {
                var dataFimExpediente = new DateTime(dataAtual.Year, dataAtual.Month, dataAtual.Day, diaDaSemana.Fim.Hours, diaDaSemana.Fim.Minutes, diaDaSemana.Fim.Seconds);
                difMinutos += Convert.ToInt32(dataFimExpediente.Subtract(dataAtual).TotalMinutes);


                // Clausio 11/01/2011
                // Caso a data seja invalida, retorna a dif em minutos calculada ate o momento
                try
                {
                    dataAtual = dateTime.ObterProximoDiaUtil(dataAtual.AddDays(1).Date, calendario, feriados);
                }
                catch
                {
                    difMinutos = 0;
                    return difMinutos;
                }

                diaDaSemana = calendario.ObterDiaDaSemana(dataAtual);
                dataAtual = new DateTime(dataAtual.Year, dataAtual.Month, dataAtual.Day, diaDaSemana.Inicio.Hours, diaDaSemana.Inicio.Minutes, diaDaSemana.Inicio.Seconds);

            }

            // Tratando se a data inicial e final são no mesmo dia
            if (dataFinal.ToString("dd/MM/yyyy") == dataAtual.ToString("dd/MM/yyyy"))//(dataFinal.Subtract(dataAtual).Days == 0)
            {
                if (dataFinal.Hour >= diaDaSemana.Fim.Hours)
                    dataFinal = new DateTime(dataAtual.Year, dataAtual.Month, dataAtual.Day, diaDaSemana.Fim.Hours, diaDaSemana.Fim.Minutes, diaDaSemana.Fim.Seconds);
                difMinutos += Convert.ToInt32(dataFinal.Subtract(dataAtual).TotalMinutes);
            }

            return difMinutos;
        }

        private DateTime AtualizaDataInicial(DateTime deadLine, DiaDaSemana diaDaSemana, CalendarioDeTrabalho calendario, List<Feriado> feriados, bool proximoDia)
        {

            //Caso seja feriado ou o horário ultrapasse o expediente
            if (proximoDia)
            {
                deadLine = deadLine.ObterProximoDiaUtil(deadLine.AddDays(1).Date, calendario, feriados);
                diaDaSemana = calendario.ObterDiaDaSemana(deadLine);
            }

            if (deadLine.Hour < 8)
                deadLine = new DateTime(deadLine.Year, deadLine.Month, deadLine.Day, diaDaSemana.Inicio.Hours, diaDaSemana.Inicio.Minutes, diaDaSemana.Inicio.Seconds);

            return deadLine;
        }

        private DateTime CalcularDataDeAtendimento(DateTime dataDeCriacao, int tempoTotalDoSLA, CalendarioDeTrabalho calendario, List<Feriado> feriados)
        {

            #region Inicializa Variáveis

            DateTime dateTime = new DateTime();
            DateTime deadLine = dataDeCriacao;
            int tempoQueSobrouParaOProximoDiaUtil = 0;
            int tempoTotalDoSLAEmMinutos = tempoTotalDoSLA;
            DiaDaSemana diaDaSemana = calendario.ObterDiaDaSemana(deadLine);

            #endregion

            #region Define início do atendimento conforme expediente

            //Caso a origem seja feriado ou o horário ultrapasse o expediente, procura o próximo dia útil
            //Caso a origem seja anterior ao expediente, iguala o horário

            if (Feriado.VerificaFeriado(deadLine, feriados) || deadLine.Hour >= diaDaSemana.Fim.Hours)
                deadLine = AtualizaDataInicial(deadLine, diaDaSemana, calendario, feriados, true);
            else
                deadLine = AtualizaDataInicial(deadLine, diaDaSemana, calendario, feriados, false);

            #endregion

            #region Verifica se o dia de início comportará todo o tempo necessário para o cumprimento do SLA
            var tempoParaConcluirAJornadaDeTrabalho = calendario.ObterTempoRestanteDaJornadaDeTrabalho(deadLine,
                  new TimeSpan(deadLine.Hour, deadLine.Minute, deadLine.Second)).TotalMinutes;

            if (tempoParaConcluirAJornadaDeTrabalho > 0)

                tempoQueSobrouParaOProximoDiaUtil = tempoTotalDoSLAEmMinutos - Convert.ToInt32(tempoParaConcluirAJornadaDeTrabalho);
            else
                tempoQueSobrouParaOProximoDiaUtil *= 60;
            #endregion

            #region Caso não haja tempo suficiente, tenta o próximo dia útil. Se não resolver, percorre os próximos dias até que encontre uma data para o prazo
            if (tempoQueSobrouParaOProximoDiaUtil > 0)
            {
                deadLine = dateTime.ObterProximoDiaUtil(deadLine.AddDays(1).Date, calendario, feriados);

                var nextWeekDay = calendario.ObterDiaDaSemana(deadLine);
                deadLine = deadLine.AddHours(nextWeekDay.Inicio.Hours);

                if (tempoQueSobrouParaOProximoDiaUtil <= Convert.ToInt32(calendario.ObterTempoRestanteDaJornadaDeTrabalho(deadLine,
                   new TimeSpan(deadLine.Hour, deadLine.Minute, deadLine.Second)).TotalMinutes))
                    return deadLine.AddMinutes(tempoQueSobrouParaOProximoDiaUtil);

                return CalcularDataDeAtendimento(deadLine, tempoQueSobrouParaOProximoDiaUtil, calendario, feriados);
            }
            #endregion

            return deadLine.AddMinutes(tempoTotalDoSLAEmMinutos);
        }

        public List<Ocorrencia> PesquisarOcorrenciaPor(string postoDeServicoId, int status, int tipoDeOcorrencia, string nomeDoCliente, DateTime dataInicial, DateTime dataFinal)
        {
            return PesquisarOcorrenciaPor(postoDeServicoId, status, tipoDeOcorrencia, nomeDoCliente, dataInicial, dataFinal); //;
        }

        public void ValidarObtencaoContratoAutorizacaoPostagemCorreios()
        {
            //O assunto não pode ter um AssuntoPaiId
            if (this.AssuntoUnidade == null)
                throw new Exception("Assunto 'Unidade de Negócio' não informado.");

            //O assunto não pode ter um AssuntoPaiId
            if (this.AssuntoUnidade.AssuntoPaiId != null)
                throw new Exception("Assunto inválido (Unidade de Negócio).");
        }

        public AutorizacaoPostagemCorreios ObtemContratoAutorizacaoPostagemCorreios(List<AutorizacaoPostagemCorreios> contratos)
        {
            switch (contratos.Count)
            {
                case 0: throw new Exception("Nenhum contrato de autorização de postagem ativo!");
                case 1: return contratos[0];
                default: throw new Exception(string.Format("Existem mais de 1 contrato de autorização de postagem ativos para a Unidade de Negócio '{0}' (Assunto)!", this.AssuntoUnidade.Nome.Trim()));
            }
        }

        /// <summary>
        /// Atualiza os assuntos da ocorrencia de acorodo com a entidade ClassificacaoAssunto.
        /// </summary>
        public bool AtualizarClassificacaoAssunto()
        {
            if (this.Id == Guid.Empty
                || this.RazaoStatus.Value == (int)StatusDaOcorrencia.Vazio
                || this.RazaoStatus.Value == (int)StatusDaOcorrencia.CanceladaSistema
                || !this.ClassificacaoAssuntoDiferente())
                return false;

            Ocorrencia ocorrenciaTemp = new Ocorrencia() { Id = this.Id, AssuntoId = new Lookup() { Id = this.AssuntoId.Id } };
            SolucaoOcorrencia solucaoOcorrencia = null;
            bool ocorrenciaEstavaFechada = (this.RazaoStatus.Value == (int)StatusDaOcorrencia.Resolvido);

            if (ocorrenciaEstavaFechada)
            {
                solucaoOcorrencia = (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.ListarSolucoesOcorrencia(this)[0];
                (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.ReabrirOcorrencia(this);
            }

            (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.Update(ocorrenciaTemp);

            if (ocorrenciaEstavaFechada)
                (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.FecharOcorrencia(ocorrenciaTemp, solucaoOcorrencia);

            return true;
        }

        public bool ClassificacaoAssuntoDiferente()
        {
            if (this.AssuntoId == null) return false;

            Assunto assuntoOcorrencia = new Assunto() { Id = this.AssuntoId.Id };
            Ocorrencia ocorrenciaTemp = new Ocorrencia() { Id = this.Id };

            List<Assunto> listaAssunto = assuntoOcorrencia.EstruturaDoAssunto();

            foreach (Assunto assuntoAux in listaAssunto)
                switch (assuntoAux.TipoDeAssunto)
                {
                    case TipoDeAssunto.Motivo:
                        ocorrenciaTemp.AssuntoMotivo = assuntoAux;
                        break;
                    case TipoDeAssunto.Parte:
                        ocorrenciaTemp.AssuntoParte = assuntoAux;
                        break;
                    case TipoDeAssunto.Problema:
                        ocorrenciaTemp.AssuntoProblema = assuntoAux;
                        break;
                    case TipoDeAssunto.Produto:
                        ocorrenciaTemp.AssuntoProduto = assuntoAux;
                        break;
                    case TipoDeAssunto.Serie:
                        ocorrenciaTemp.AssuntoSerie = assuntoAux;
                        break;
                    case TipoDeAssunto.TipoDeProduto:
                        ocorrenciaTemp.AssuntoTipo = assuntoAux;
                        break;
                    case TipoDeAssunto.UnidadeDeNegocio:
                        ocorrenciaTemp.AssuntoUnidade = assuntoAux;
                        break;
                    case TipoDeAssunto.Vazio:
                        break;
                }


            if (this.AssuntoMotivo == null)
            {
                if (ocorrenciaTemp.AssuntoMotivo != null) return true;
            }
            else if (!this.AssuntoMotivo.Equals(ocorrenciaTemp.AssuntoMotivo)) return true;

            if (this.AssuntoParte == null)
            {
                if (ocorrenciaTemp.AssuntoParte != null) return true;
            }
            else if (!this.AssuntoParte.Equals(ocorrenciaTemp.AssuntoParte)) return true;

            if (this.AssuntoProblema == null)
            {
                if (ocorrenciaTemp.AssuntoProblema != null) return true;
            }
            else if (!this.AssuntoProblema.Equals(ocorrenciaTemp.AssuntoProblema)) return true;

            if (this.AssuntoProduto == null)
            {
                if (ocorrenciaTemp.AssuntoProduto != null) return true;
            }
            else if (!this.AssuntoProduto.Equals(ocorrenciaTemp.AssuntoProduto)) return true;

            if (this.AssuntoSerie == null)
            {
                if (ocorrenciaTemp.AssuntoSerie != null) return true;
            }
            else if (!this.AssuntoSerie.Equals(ocorrenciaTemp.AssuntoSerie)) return true;

            if (this.AssuntoTipo == null)
            {
                if (ocorrenciaTemp.AssuntoTipo != null) return true;
            }
            else if (!this.AssuntoTipo.Equals(ocorrenciaTemp.AssuntoTipo)) return true;

            if (this.AssuntoUnidade == null)
            {
                if (ocorrenciaTemp.AssuntoUnidade != null) return true;
            }
            else if (!this.AssuntoUnidade.Equals(ocorrenciaTemp.AssuntoUnidade)) return true;

            return false;
        }

        public void EfetuaReincidencia(ref Ocorrencia ocorrenciaPai)
        {
            // Atualiza identificador de reincidência
            ocorrenciaPai = this.OcorrenciaPai;

            if (this.Cliente == null) return;
            if (this.Produto == null) return;
            if (this.Produto.DadosFamiliaComercial == null) return;
            if (this.Produto.LinhaComercial == null) return;

            // Obtém usuários
            var usuarios = (new CRM2013.Domain.Servicos.RepositoryService()).Usuario.ListarPorFamiliaComercial(this.Produto);
            if (usuarios.Count == 0) return;

            // Obtém o Email "De"
            var emailDe = SDKore.Configuration.ConfigurationManager.GetSettingValue("ID_EMAIL_CORPORATIVO");
            if (emailDe == null) return;

            foreach (Usuario usuario in usuarios)
            {
                Email email = new Email();
                email.ReferenteAId = this.Id;
                email.ReferenteAType = "incident";
                email.ReferenteAName = this.Nome;
                email.Assunto = "Produto com OS Reincidente";
                email.Mensagem = string.Format("<b>Cliente</b>: {0}<br><b>Produto</b>: {1} {2}<br><b>Nota Fiscal</b>: {3}<br><b>OS Existente</b>: {4}", this.Cliente.Nome, this.Produto.Codigo, this.Produto.Nome, this.NotaFiscalFatura, this.Numero);
                email.De = new Lookup[1];
                email.De[0] = new Lookup { Id = new Guid(emailDe), Type = SDKore.Crm.Util.Utility.GetEntityName<Model.Usuario>() };
                email.Para = new Lookup[1];
                email.Para[0] = new Lookup { Id = usuario.Id, Type = SDKore.Crm.Util.Utility.GetEntityName<Model.Usuario>() };

                email.Direcao = false;
                email.ID = (new CRM2013.Domain.Servicos.RepositoryService()).Email.Create(email);

                (new CRM2013.Domain.Servicos.RepositoryService()).Email.EnviarEmail(email.ID.Value);
            }

        }

        public bool EfetuaRedundancia()
        {
            if (this.Origem == 200004 || this.Origem == 200006) return (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.ObtemReduntanteASTEC(this) != null;
            return false;
        }

        public decimal ObterValorDeServico()
        {
            decimal valor = 0;
            decimal valorPercentual = 0;

            if (this.AutorizadaId == null || this.ProdutoId == null)
                return 0;

            #region Primeiro calculo

            valor = (new CRM2013.Domain.Servicos.RepositoryService()).ValorDoServicoPorPosto.ObterMaiorValorPor(this.Autorizada, this.Produto);
            if (valor != decimal.MinValue)
                return valor;

            valor = (new CRM2013.Domain.Servicos.RepositoryService()).ValorDoServicoPorPosto.ObterMaiorValorPorLinhaComercialDoProduto(this.Autorizada, this.Produto);
            if (valor != decimal.MinValue)
                return valor;

            #endregion

            #region Segundo calculo

            if (this.Produto.ValorDaMaoDeObra.HasValue)
            {
                valor = (new CRM2013.Domain.Servicos.RepositoryService()).TipoPosto.ObterMaiorValorPor(this.Autorizada);

                if (valor == decimal.MinValue)
                    valor = 0;

                valorPercentual = 1 + valor / 100;

                valor = this.Produto.ValorDaMaoDeObra.Value * valorPercentual;
                return valor;
            }

            #endregion

            #region Terceiro calculo

            if (this.Id != Guid.Empty)
            {
                valor = (new CRM2013.Domain.Servicos.RepositoryService()).ValorDoServico.ObterMaiorValorPor(this);
                if (valor != decimal.MinValue)
                {
                    var valorPosto = (new CRM2013.Domain.Servicos.RepositoryService()).TipoPosto.ObterMaiorValorPor(this.Autorizada);

                    if (valorPosto == decimal.MinValue)
                        valorPosto = 0;

                    valorPercentual = 1 + valorPosto / 100;

                    valor = valor * valorPercentual;

                    return valor;
                }
            }

            #endregion

            #region Default

            return 0;

            #endregion
        }

        public bool EfetuaValidacaoDePedidoAstec()
        {
            string mensagem = string.Empty;

            if (this.Autorizada == null) mensagem = "Não tem Autorizada cadastrada!";
            else if (this.Autorizada.TransportadoraASTEC == null) mensagem = string.Format("Autorizada: {0} - {1} não possui transportadora configurada para gerar pedidos", this.Autorizada.CodigoMatriz, this.Autorizada.Nome);
            else if (this.Produto == null) mensagem = string.Format("Ocorrência({0}) sem produto associado", this.Numero);
            else if (this.Produto.DadosFamiliaComercial == null) mensagem = string.Format("Produto {0} - {1} não possui Família Comercial configurada para gerar pedidos", this.Produto.Codigo, this.Produto.Nome);
            else if (this.Produto.LinhaComercial == null) mensagem = string.Format("Produto {0} - {1} não possui Segmento configurada para gerar pedidos", this.Produto.Codigo, this.Produto.Nome);
            else if (this.Produto.LinhaComercial.Estabelecimento == null) mensagem = string.Format("Produto {0} - {1} não possui Estabelecimento configurada para gerar pedidos", this.Produto.Codigo, this.Produto.Nome);

            if (mensagem != string.Empty)
            {   
                return false;
            }
            return this.Autorizada.DiaDePedidoAssistenciaTecnica;
        }

        public int VerificaAlteracaoDeStatusPorDiagnosticos()
        {
            int status = int.MaxValue;

            if (this.Id != Guid.Empty)
            {
                List<Diagnostico> diagnosticos = (new CRM2013.Domain.Servicos.RepositoryService()).Diagnostico.ListarPor(this);
                foreach (Diagnostico diagnostico in diagnosticos)
                {
                    if (diagnostico.RazaoStatus.Value == (int)StatusDoDiagnostico.AguardandoPeca || diagnostico.RazaoStatus.Value == (int)StatusDoDiagnostico.PedidoSolicitadoAoEms)
                        if ((int)diagnostico.RazaoStatus < status)
                            status = (int)diagnostico.RazaoStatus;

                    if (status == 1)
                        break;
                }
            }

            return status;
        }

        public void FinalizarDiagnosticos(Ocorrencia ocorrencia)
        {
            var lstDiagnosticos = (new RepositoryService()).Diagnostico.ListarDiagnosticoPortalPor(ocorrencia);

            foreach (var item in lstDiagnosticos)
            {
                if (item.StatusDiagnostico == StatusDoDiagnostico.AguardandoConserto)
                {
                    item.RazaoStatus = (int)StatusDoDiagnostico.ConsertoRealizado;
                    new RepositoryService(OrganizationName, IsOffline).Diagnostico.Update(item);
                }
            }
        }

        public bool GarantiaPorContratoEstaVigente()
        {
            if (string.IsNullOrEmpty(this.ProdutosDoCliente))
            {
                return false;
            }

            return (new CRM2013.Domain.Servicos.RepositoryService()).Produto.ProdutoPossuiGarantiaEspecificaDentroDaVigenciaPor(this.ProdutosDoCliente);
        }

        public decimal ObterSomaPagamentos(PagamentoServico pagamentoalterado)
        {
            decimal total = 0;
            List<PagamentoServico> pagamentos = (new RepositoryService()).PagamentoServico.ListarPor(this);
            foreach (PagamentoServico pagamento in pagamentos)
            {
                if (pagamento.Valor.HasValue)
                    total += Convert.ToDecimal(pagamento.Valor);
            }

            if (pagamentoalterado != null)
            {
                total = total - Convert.ToDecimal(pagamentoalterado.Valor);
            }

            return total;
        }
        #endregion

    }
}
