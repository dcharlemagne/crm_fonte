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
    public class OcorrenciaBase : DomainBase
    {
        #region Atributos
        [LogicalAttribute("title")]
        public String Nome { get; set; }
        [LogicalAttribute("new_data_ajuste_posto")]
        public DateTime? DataEnvioAjustePosto { get; set; }
        [LogicalAttribute("new_data_fabricacao_produto")]
        public DateTime? DataFabricacaoProduto { get; set; }
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
        [LogicalAttribute("new_reincidenteid")]
        public Lookup OcorrenciaPai { get; set; }
        [LogicalAttribute("casetypecode")]
        public int? TipoDeOcorrencia { get; set; }
        public string Tipo 
        {
            get { return (TipoDeOcorrencia.HasValue ? ((Enum.TipoDeOcorrencia)TipoDeOcorrencia).ToString() : string.Empty); }
            set { }
        }
        [LogicalAttribute("new_data_hora_conclusao")]
        public DateTime? DataDeConclusao { get; set; }
        public bool EmAndamento
        {
            get { return (DataDeConclusao.HasValue ? true : false); }
            set { }
        }

        [LogicalAttribute("customerid")]
        public Lookup Cliente { get; set; }

        [LogicalAttribute("new_empresa_executanteid")]
        public Lookup EmpresaExecutanteId { get; set; }

        [LogicalAttribute("new_acao_final2")]
        public Lookup AcaoFinal { get; set; }

        [LogicalAttribute("new_autorizadaid")]
        public Lookup Autorizada { get; set; }

        [LogicalAttribute("productid")]
        public Lookup Produto { get; set; }

        [LogicalAttribute("contractid")]
        public Lookup Contrato { get; set; }

        [LogicalAttribute("contractdetailid")]
        public Lookup LinhaDeContrato { get; set; }

        [LogicalAttribute("new_tecnico_visitaid")]
        public Lookup TecnicoDaVisita { get; set; }

        [LogicalAttribute("new_tecnico_responsavelid")]
        public Lookup TecnicoResponsavel { get; set; }

        [LogicalAttribute("new_solicitanteid")]
        public Lookup Solicitante { get; set; }

        [LogicalAttribute("new_localidadeid")]
        public Lookup Localidade { get; set; }
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
        public Lookup ReferenciaExtratoPagamento { get; set; }
        [LogicalAttribute("new_acaoid")]
        public Lookup Acao { get; set; }
        [LogicalAttribute("subjectid")]
        public Lookup Assunto { get; set; }
        [LogicalAttribute("new_unidade_negocio_astec")]
        public Lookup UnidadeDeNegocioAstec { get; set; }

        [LogicalAttribute("new_assunto_unidadeid")]
        public Lookup AssuntoUnidade { get; set; }
        [LogicalAttribute("new_assunto_tipoid")]
        public Lookup AssuntoTipo { get; set; }
        public Lookup AssuntoSerie { get; set; }
        [LogicalAttribute("new_assunto_produtoid")]
        public Lookup AssuntoProduto { get; set; }
        [LogicalAttribute("new_assunto_problemaid")]
        public Lookup AssuntoProblema { get; set; }
        [LogicalAttribute("new_assunto_parteid")]
        public Lookup AssuntoParte { get; set; }
        [LogicalAttribute("new_assunto_motivoid")]
        public Lookup AssuntoMotivo { get; set; }
        [LogicalAttribute("new_solucao_asstec")]
        public int? AcaoAssistenciaValue { get; set; }
        public AcaoAssistencia? new_solucao_asstec
        {
            get { return (AcaoAssistenciaValue.HasValue ? (AcaoAssistencia)AcaoAssistenciaValue.Value : AcaoAssistencia.Vazio); }
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
        public StatusDaOcorrencia StatusDaOcorrencia
        {
            get { return (RazaoStatus.HasValue ? (StatusDaOcorrencia)RazaoStatus.Value : StatusDaOcorrencia.Vazio); }
            set { }
        }
        //Fim dos dados na Nota Fiscal do Consumidor
        #endregion

        #region Contrutores
        private RepositoryService RepositoryService { get; set; }

        public OcorrenciaBase()
        {
        }

        public OcorrenciaBase(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public OcorrenciaBase(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

    }
}
