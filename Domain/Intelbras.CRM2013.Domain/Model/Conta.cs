using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    [LogicalEntity("account")]
    public class Conta : IntegracaoBase
    {
        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public Conta() { }

        public Conta(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Conta(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("accountid")]
        public Guid? ID { get; set; }

        //[LogicalAttribute("accountcategorycode")]
        //public Int32? TipoCliente { get; set; }

        [LogicalAttribute("accountnumber")]
        public String CodigoMatriz { get; set; }

        [LogicalAttribute("address1_addresstypecode")]
        public Int32? TipoEndereco { get; set; }

        [LogicalAttribute("address1_city")]
        public String Endereco1Cidade { get; set; }

        [LogicalAttribute("address1_country")]
        public String Endereco1Pais1 { get; set; }

        [LogicalAttribute("new_altera_endereco_padrao")]
        public String EnderecoPadrao { get; set; }

        [LogicalAttribute("address1_county")]
        public String Endereco1Bairro1 { get; set; }

        //[LogicalAttribute("address1_freighttermscode")]
        //public Int32? CondicoesFrete { get; set; }

        [LogicalAttribute("address1_line1")]
        public String Endereco1Rua1 { get; set; }

        [LogicalAttribute("address1_line2")]
        public String Endereco1Bairro { get; set; }

        [LogicalAttribute("address1_line3")]
        public String Endereco1Complemento { get; set; }

        [LogicalAttribute("address1_name")]
        public String Endereco1Nome { get; set; }

        [LogicalAttribute("address1_postalcode")]
        public String Endereco1CEP { get; set; }

        [LogicalAttribute("address1_postofficebox")]
        public String Endereco1CaixaPostal { get; set; }

        //[LogicalAttribute("address1_shippingmethodcode")]
        //public Int32? Endereco1MetodoEntrega { get; set; }

        [LogicalAttribute("address1_stateorprovince")]
        public String Endereco1Estado { get; set; }

        [LogicalAttribute("address1_telephone1")]
        public String TelefoneEndereco { get; set; }

        [LogicalAttribute("address2_telephone1")]
        public String TelefoneEnderecoCobranca { get; set; }

        [LogicalAttribute("address2_name")]
        public String Endereco2Nome { get; set; }

        [LogicalAttribute("address2_addresstypecode")]
        public Int32? Endereco2TipoEndereco { get; set; }

        [LogicalAttribute("address2_city")]
        public String Endereco2Cidade { get; set; }

        [LogicalAttribute("address2_country")]
        public String Endereco2Pais2 { get; set; }

        [LogicalAttribute("address2_county")]
        public String Endereco2Bairro2 { get; set; }

        [LogicalAttribute("address2_line1")]
        public String Endereco2Rua2 { get; set; }

        [LogicalAttribute("address2_line2")]
        public String Endereco2Bairro { get; set; }

        [LogicalAttribute("address2_line3")]
        public String Endereco2Complemento { get; set; }

        [LogicalAttribute("address2_postalcode")]
        public String Endereco2CEP { get; set; }

        [LogicalAttribute("address2_postofficebox")]
        public String Endereco2CaixaPostal { get; set; }

        [LogicalAttribute("address2_stateorprovince")]
        public String Endereco2Estado { get; set; }

        [LogicalAttribute("creditlimit")]
        public Decimal? LimiteCredito { get; set; }

        [LogicalAttribute("creditonhold")]
        public Boolean? SuspensaoCredito { get; set; }

        [LogicalAttribute("customertypecode")]
        public Int32? TipoRelacao { get; set; }

        [LogicalAttribute("defaultpricelevelid")]
        public Lookup ListaPreco { get; set; }

        [LogicalAttribute("description")]
        public String Descricao { get; set; }

        //[LogicalAttribute("donotbulkemail")]
        //public Boolean? NaoPermitirEmailMassa { get; set; }

        //[LogicalAttribute("donotemail")]
        //public Boolean? NaoPermitirEmail { get; set; }

        //[LogicalAttribute("donotfax")]
        //public Boolean? NaoPermitirFax { get; set; }

        //[LogicalAttribute("donotphone")]
        //public Boolean? NaoPermitirTelefonemas { get; set; }

        //[LogicalAttribute("donotpostalmail")]
        //public Boolean? NaoPermitirCorrespondencia { get; set; }

        //[LogicalAttribute("donotsendmm")]
        //public Boolean? EnviarMaterialMarketing { get; set; }

        [LogicalAttribute("emailaddress1")]
        public String Email { get; set; }

        [LogicalAttribute("fax")]
        public String Fax { get; set; }

        [LogicalAttribute("industrycode")]
        public Int32? Setor { get; set; }

        [LogicalAttribute("new_grupo_clienteid")]
        public Lookup Grupo { get; set; }
        public GrupoDoCliente GrupoCliente
        {
            get
            {
                GrupoDoCliente grupo = null;
                if (Grupo != null && this.Id != Guid.Empty)
                    grupo = (new Domain.Servicos.RepositoryService()).GrupoDoCliente.ObterPor(this.Id);
                return grupo;
            }

            set { }
        } 

        [LogicalAttribute("itbc_representante")]
        public Lookup Representante { get; set; }

        //private Contato representante = null;
        //[LogicalAttribute("new_representanteid")]
        //public Contato Representante
        //{
        //    get
        //    {
        //        if (representante == null)
        //            representante = (new Domain.Servicos.RepositoryService()).Contato.ObterRepresentatePor(this);
        //        return representante;
        //        //return new Lookup(grupo.Id, grupo.Nome, "new_grupo_cliente");
        //    }

        //    set { representante = value; }
        //}
        public decimal LimiteDeCreditoIntelbras { get; set; }
        public decimal LimiteDeCreditoDisponivelIntelbras { get; set; }
        public decimal LimiteDeCreditoSupplierCard { get; set; }
        public decimal LimiteDeCreditoDisponivelSupplierCard { get; set; }
        [LogicalAttribute("itbc_address1_city")]
        public Lookup Endereco1Municipioid { get; set; }
        public String Endereco1Municipio { get; set; }

        [LogicalAttribute("itbc_address1_country")]
        public Lookup Endereco1Pais { get; set; }

        [LogicalAttribute("itbc_address1_number")]
        public String Endereco1Numero { get; set; }

        [LogicalAttribute("itbc_address1_stateorprovince")]
        public Lookup Endereco1Estadoid { get; set; }

        [LogicalAttribute("itbc_address1_street")]
        public String Endereco1Rua { get; set; }

        [LogicalAttribute("itbc_address2_city")]
        public Lookup Endereco2Municipioid { get; set; }

        [LogicalAttribute("itbc_address2_country")]
        public Lookup Endereco2Pais { get; set; }

        [LogicalAttribute("itbc_address2_number")]
        public String Endereco2Numero { get; set; }

        [LogicalAttribute("itbc_address2_stateorprovince")]
        public Lookup Endereco2Estadoid { get; set; }

        [LogicalAttribute("itbc_address2_street")]
        public String Endereco2Rua { get; set; }

        [LogicalAttribute("itbc_agencia")]
        public String Agencia { get; set; }

        [LogicalAttribute("itbc_agenteretencao")]
        public Boolean? AgenteRetencao { get; set; }

        [LogicalAttribute("itbc_apuracaodebeneficiosecompromissos")]
        public int? ApuracaoBeneficiosCompromissos { get; set; }

        [LogicalAttribute("itbc_atacadistavarejista")]
        public Boolean? VendasParaAtacadistaVarejista { get; set; }

        [LogicalAttribute("itbc_atividadeeconmicaramodeatividade")]
        public String RamoAtividadeEconomica { get; set; }

        [LogicalAttribute("itbc_banco")]
        public String Banco { get; set; }

        [LogicalAttribute("itbc_calcula_multa")]
        public Boolean? CalculaMulta { get; set; }

        [LogicalAttribute("itbc_classificacaoid")]
        public Lookup Classificacao { get; set; }

        [LogicalAttribute("itbc_coberturageografica")]
        public String CoberturaGeografica { get; set; }

        [LogicalAttribute("itbc_codigosuframa")]
        public String CodigoSuframa { get; set; }

        [LogicalAttribute("itbc_condicao_pagamento")]
        public Lookup CondicaoPagamento { get; set; }

        [LogicalAttribute("itbc_conta_corrente")]
        public String ContaCorrente { get; set; }

        [LogicalAttribute("itbc_contribuinteicms")]
        public Boolean? ContribuinteICMS { get; set; }

        [LogicalAttribute("itbc_cpfoucnpj")]
        public String CpfCnpj { get; set; }

        [LogicalAttribute("itbc_datadeconstituio")]
        public DateTime? DataConstituicao { get; set; }

        [LogicalAttribute("itbc_dataadesao")]
        public DateTime? DataAdesao { get; set; }

        [LogicalAttribute("itbc_datadeimplantacao")]
        public DateTime? DataImplantacao { get; set; }

        [LogicalAttribute("itbc_datadevencimentoconcessao")]
        public DateTime? DataVenctoConcessao { get; set; }

        [LogicalAttribute("itbc_datalimitedecredito")]
        public DateTime? DataLimiteCredito { get; set; }

        [LogicalAttribute("itbc_descontocat")]
        public Decimal? DescontoCAT { get; set; }

        [LogicalAttribute("itbc_diasdeatraso")]
        public Decimal? DiasAtraso { get; set; }

        [LogicalAttribute("itbc_distfontereceita")]
        public Boolean? DistribuicaoFonteReceita { get; set; }

        [LogicalAttribute("itbc_distribuidor_principal")]
        public Lookup DistribuidorPrincipal { get; set; }

        [LogicalAttribute("itbc_docidentidade")]
        public String DocIdentidade { get; set; }

        [LogicalAttribute("itbc_embarquevia")]
        public String EmbarqueVia { get; set; }

        [LogicalAttribute("itbc_emissordocidentidade")]
        public String EmissorIdentidade { get; set; }

        [LogicalAttribute("itbc_emitebloqueto")]
        public Boolean? EmiteBloqueto { get; set; }

        [LogicalAttribute("itbc_espaco_fisico_qualificado")]
        public String EspacoFisicoQualificado { get; set; }

        [LogicalAttribute("itbc_exclusividade")]
        public Boolean? Exclusividade { get; set; }

        [LogicalAttribute("itbc_formadetributacao")]
        public Int32? FormaTributacao { get; set; }

        [LogicalAttribute("itbc_gera_aviso_credito")]
        public Boolean? GeraAvisoCredito { get; set; }

        [LogicalAttribute("itbc_GuidCRM40")]
        public String GUIDCRM40 { get; set; }

        [LogicalAttribute("itbc_historico")]
        public String Historico { get; set; }

        [LogicalAttribute("itbc_incoterm")]
        public String Incoterm { get; set; }

        //[LogicalAttribute("itbc_indicada_icon")]
        //public Boolean? IndicadaICON { get; set; }

        //[LogicalAttribute("itbc_indicada_icorp")]
        //public Boolean? IndicadaICORP { get; set; }

        //[LogicalAttribute("itbc_indicada_inet")]
        //public Boolean? IndicadaINET { get; set; }

        //[LogicalAttribute("itbc_indicada_isec")]
        //public Boolean? IndicadaISEC { get; set; }

        [LogicalAttribute("itbc_inscricaoestadual")]
        public String InscricaoEstadual { get; set; }

        [LogicalAttribute("itbc_inscricaomunicipal")]
        public String InscricaoMunicipal { get; set; }

        [LogicalAttribute("itbc_intencaoapoio")]
        public String IntencaoApoio { get; set; }

        [LogicalAttribute("itbc_localembarque")]
        public String LocalEmbarque { get; set; }

        [LogicalAttribute("itbc_matrizoufilial")]
        public Int32? TipoConta { get; set; }

        [LogicalAttribute("itbc_metodo_comercializacao_produtos")]
        public String MetodoComercializacaoProduto { get; set; }

        [LogicalAttribute("itbc_modalidade")]
        public Int32? Modalidade { get; set;} 

        [LogicalAttribute("itbc_modelooperacaofiliais")]
        public String ModeloOperacaoFiliais { get; set; }

        [LogicalAttribute("itbc_natureza")]
        public Int32? Natureza { get; set; }

        [LogicalAttribute("itbc_nomeabreviado")]
        public String NomeAbreviado { get; set; }

        [LogicalAttribute("itbc_nomefantasia")]
        public String NomeFantasia { get; set; }

        [LogicalAttribute("itbc_numdecolaboradores")]
        public Int32? NumeroColaboradores { get; set; }

        [LogicalAttribute("itbc_numdevendedores")]
        public Int32? NumeroVendedores { get; set; }

        [LogicalAttribute("itbc_numerorevendasativas")]
        public Int32? NumeroRevendasAtivas { get; set; }

        [LogicalAttribute("itbc_numerorevendasinativas")]
        public String NumeroRevendasInativas { get; set; }

        [LogicalAttribute("itbc_numtecnicossuporte")]
        public Int32? NumeroTecnicosSuporte { get; set; }

        [LogicalAttribute("itbc_obsnf")]
        public String ObservacoesNF { get; set; }

        [LogicalAttribute("itbc_obspedido")]
        public String ObservacoesPedido { get; set; }

        [LogicalAttribute("itbc_optantesuspensaoipi")]
        public Boolean? OptanteSuspensaoIPI { get; set; }

        [LogicalAttribute("itbc_outrafontereceita")]
        public String OutraFonteReceita { get; set; }

        [LogicalAttribute("itbc_participa_do_programa")]
        public Int32? ParticipantePrograma { get; set; }

        [LogicalAttribute("itbc_participapcimotivo")]
        public int? MotivoParticipantePrograma { get; set; }

        [LogicalAttribute("itbc_perfilrevendasdodistribuidor")]
        public String PerfilRevendaDistribuidor { get; set; }

        [LogicalAttribute("itbc_piscofinsporunidade")]
        public Boolean? PISCOFINSPorUnidade { get; set; }

        [LogicalAttribute("itbc_portador")]
        public Lookup Portador { get; set; }

        [LogicalAttribute("itbc_possuiestruturacompleta")]
        public Int32? PossuiEstruturaCompleta { get; set; }

        [LogicalAttribute("itbc_possuifiliais")]
        public Int32? PossuiFiliais { get; set; }

        [LogicalAttribute("itbc_posvendaid")]
        public Lookup NivelPosVendas { get; set; }

        [LogicalAttribute("itbc_prazomediocompras")]
        public Double? PrazoMedioCompras { get; set; }

        [LogicalAttribute("itbc_prazomediovendas")]
        public Double? PrazoMedioVendas { get; set; }

        [LogicalAttribute("itbc_quantasfiliais")]
        public Int32? QuantasFiliais { get; set; }

        [LogicalAttribute("itbc_ramal_fax")]
        public String RamalFax { get; set; }

        [LogicalAttribute("itbc_ramaloutrotelefone")]
        public String RamalOutroTelefone { get; set; }

        [LogicalAttribute("itbc_ramaltelefoneprincipal")]
        public String RamalTelefonePrincipal { get; set; }

        [LogicalAttribute("itbc_recebe_informacao_sci")]
        public Boolean? RecebeInformacaoSCI { get; set; }

        [LogicalAttribute("itbc_recebenfe")]
        public Boolean? RecebeNFE { get; set; }

        [LogicalAttribute("itbc_receitapadraoid")]
        public Lookup ReceitaPadrao { get; set; }

        [LogicalAttribute("itbc_saldodecredito")]
        public Decimal? SaldoCredito { get; set; }

        [LogicalAttribute("itbc_softwaredenegocios")]
        public String SoftwareNegocios { get; set; }

        [LogicalAttribute("itbc_subclassificacaoid")]
        public Lookup Subclassificacao { get; set; }

        [LogicalAttribute("itbc_substituicaotributaria")]
        public String SubstituicaoTributaria { get; set; }

        [LogicalAttribute("itbc_tipodeconstituicao")]
        public int? TipoConstituicao { get; set; }

        [LogicalAttribute("itbc_tipodeembalagem")]
        public String TipoEmbalagem { get; set; }

        [LogicalAttribute("itbc_transportadora")]
        public Lookup Transportadora { get; set; }

        [LogicalAttribute("itbc_transportadoraredespacho")]
        public Lookup TransportadoraRedespacho { get; set; }

        [LogicalAttribute("itbc_transportadora_assistencia_tecnica")]
        public Lookup TransportadoraASTEC { get; set; }

        [LogicalAttribute("itbc_valormediocomprasmensais")]
        public Decimal? ValorMedioComprasMensais { get; set; }

        [LogicalAttribute("itbc_valormediovendasmensais")]
        public Decimal? ValorMedioVendasMensais { get; set; }

        //[LogicalAttribute("lastusedincampaign")]
        //public DateTime? DataUltimaInclusaoCampanha { get; set; }

        [LogicalAttribute("name")]
        public String RazaoSocial { get; set; }
        [LogicalAttribute("name")]
        public String Nome { get; set; }

        [LogicalAttribute("numberofemployees")]
        public Int32? NumeroFuncionarios { get; set; }

        [LogicalAttribute("originatingleadid")]
        public Lookup ClientePotencialOriginador { get; set; }
  
        [LogicalAttribute("ownershipcode")]
        public Int32? Propriedade { get; set; }

        [LogicalAttribute("parentaccountid")]
        public Lookup ContaPrimaria { get; set; }

        //[LogicalAttribute("paymenttermscode")]
        //public Int32? CondicoesPagamento { get; set; }

        //[LogicalAttribute("preferredappointmentdaycode")]
        //public Int32? DiaPreferencial { get; set; }

        //[LogicalAttribute("preferredappointmenttimecode")]
        //public Int32? HorarioPreferencial { get; set; }

        //[LogicalAttribute("preferredcontactmethodcode")]
        //public Int32? FormaPreferencialContato { get; set; }

        //[LogicalAttribute("preferredequipmentid")]
        //public Lookup InstalacoesEquipPreferenciais { get; set; }

        //[LogicalAttribute("preferredserviceid")]
        //public Lookup ServicoPreferencial { get; set; }

        //[LogicalAttribute("preferredsystemuserid")]
        //public Lookup UsuarioPreferencial { get; set; }

        [LogicalAttribute("primarycontactid")]
        public Lookup ContatoPrimario { get; set; }

        [LogicalAttribute("revenue")]
        public Decimal? ReceitaAnual { get; set; }

        [LogicalAttribute("sic")]
        public String CNAE { get; set; }

        [LogicalAttribute("telephone1")]
        public String Telefone { get; set; }

        [LogicalAttribute("telephone2")]
        public String TelefoneAlternativo { get; set; }

        [LogicalAttribute("territoryid")]
        public Lookup Regiao { get; set; }

        [LogicalAttribute("itbc_cnaeid")]
        public Lookup CnaeId { get; set; }

        //[LogicalAttribute("tickersymbol")]
        //public String SimboloAcao { get; set; }

        //[LogicalAttribute("TipoProprietario")]
        //public String TipoProprietario { get; set; }

        //[LogicalAttribute("transactioncurrencyid")]
        //public Lookup Moeda { get; set; }

        [LogicalAttribute("websiteurl")]
        public String Site { get; set; }

        [LogicalAttribute("itbc_origemconta")]
        public int? OrigemConta { get; set; }

        [LogicalAttribute("itbc_numeropassaporte")]
        public string NumeroPassaporte { get; set; }

        [LogicalAttribute("itbc_statusintegracaosefaz")]
        public int? StatusIntegracaoSefaz { get; set; }

        [LogicalAttribute("itbc_datahoraintegracaosefaz")]
        public DateTime? DataHoraIntegracaoSefaz { get; set; }

        [LogicalAttribute("itbc_dataultimosellout")]
        public DateTime? DataUltimoSelloutRevenda { get; set; }

        [LogicalAttribute("itbc_regimeapuracao")]
        public string RegimeApuracao { get; set; }

        [LogicalAttribute("itbc_databaixacontribuinte")]
        public DateTime? DataBaixaContribuinte { get; set; }

        [LogicalAttribute("itbc_acaocrm")]
        public bool IntegrarNoPlugin { get; set; }

        [LogicalAttribute("itbc_IsAstec")]
        public bool? AssistenciaTecnica { get; set; }

        [LogicalAttribute("itbc_escolheudistrforasellout")]
        public bool? EscolheuDistribuidorSemSellout { get; set; }

        [LogicalAttribute("itbc_figuranosite")]
        public bool? FiguraNoSite { get; set; }

        [LogicalAttribute("new_divulgada_site")]
        public bool? DivulgadaNoSite { get; set; }

        [LogicalAttribute("itbc_PerfilAstec")]
        public int? PerfilAssistenciaTecnica { get; set; }

        [LogicalAttribute("itbc_TabelaPrecoAstec")]
        public int? TabelaPrecoAstec { get; set; }

         [LogicalAttribute("itbc_NomeAbrevMatrizEconomica")]
        public string NomeAbreviadoMatrizEconomica { get; set; }

        [LogicalAttribute("itbc_adesaopcirealizadapor")]
        public string AdesaoAoPCIRealizadaPor { get; set; }
        [LogicalAttribute("new_mercado_atuacao_telecom")]
        public bool? AtuaTelecom { get; set; }

        [LogicalAttribute("new_mercado_atuacao_redes")]
        public bool? AtuaRedes { get; set; }

        [LogicalAttribute("new_mercado_atuacao_seguranca")]
        public bool? AtuaSeguranca { get; set; }

        [LogicalAttribute("new_perfil_empresa_fidelidade")]
        public int? PerfilEmpresa { get; set; }
        [LogicalAttribute("new_integracao_revenda_site")]
        public int? IntegacaoRevendaSite { get; set; }

        public string DispositivoLegal { get; set; }
        public string InscricaoAuxiliarDeSubstituicao { get; set; }
        public bool OptanteDeSuspensaoDeIPI { get; set; }
        public string PisConfinsPorUnidade { get; set; }
        Domain.Enum.FormaDeTributacao FormaDeTributacao { get; set; }
        [LogicalAttribute("new_geracao_pedido_posto")]
        public int? GeracaoPedido { get; set; }
        
        [LogicalAttribute("new_posto_servico")]
        public Boolean? AcessoPortalASTEC { get; set; }

        [LogicalAttribute("new_prestador_servico_isol")]
        public Boolean? PrestacaoServicoIsol { get; set; }

        public bool DiaDePedidoAssistenciaTecnica
        {
            get
            {
                if (!this.GeracaoPedido.HasValue)
                {
                    throw new ArgumentNullException("GeracaoPedido", "Atributo geração pedido esta vazio!");
                }

                //NAO ESQUECE DE TIRAR
                //return true;

                var dayOfWeek = DateTime.Now.DayOfWeek;

                switch (this.GeracaoPedido)
                {
                    case 1:
                        return (dayOfWeek == DayOfWeek.Monday || dayOfWeek == DayOfWeek.Wednesday);

                    case 2:
                        return (dayOfWeek == DayOfWeek.Tuesday || dayOfWeek == DayOfWeek.Thursday);

                    case 3:
                        return (dayOfWeek == DayOfWeek.Monday || dayOfWeek == DayOfWeek.Wednesday || dayOfWeek == DayOfWeek.Friday);

                    case 4:
                        return (dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday);

                    case 5:
                        return (dayOfWeek == DayOfWeek.Monday || dayOfWeek == DayOfWeek.Thursday);

                    case 6:
                        return (dayOfWeek == DayOfWeek.Monday);

                    default:
                        return false;
                }
            }
        }

        [LogicalAttribute("itbc_categorizar")]
        public int? Categorizar { get; set; }

        [LogicalAttribute("itbc_categoria")]
        public Lookup Categoria { get; set; }
        public CategoriaB2B CategoriaB2B { get; set; }

        [LogicalAttribute("modifiedon")]
        public DateTime? DataModificacao { get; set; }

        [LogicalAttribute("itbc_integraintelbraspontua")]
        public Boolean? IntegraIntelbrasPontua { get; set; }

        [LogicalAttribute("itbc_temdireitovmc")]
        public Boolean? TemDrireitoVMC { get; set; }

        [LogicalAttribute("itbc_statusenviovmc")]
        public int? StatusEnvioVMC { get; set; }

        [LogicalAttribute("itbc_dataultimoenviovmc")]
        public DateTime? DataUltimoEnvioVMC { get; set; }

        [LogicalAttribute("itbc_canaldevenda")]
        public int? CanaldeVenda { get; set; }

        [LogicalAttribute("itbc_identificaodaconta")]
        public int? IdentificacaoConta { get; set; }

        [LogicalAttribute("itbc_codigorepresentante")]
        public int? CodigoRepresentante { get; set; }

        [LogicalAttribute("itbc_pode_pontuar_sellin")]
        public Boolean? PodePontuarSellin { get; set; }
        
        [LogicalAttribute("itbc_data_atualizacao_perfil")]
        public DateTime? DataAtualizacaoPerfil { get; set; }

        [LogicalAttribute("itbc_usuario_atualizacao_perfil")]
        public string UsuarioAtualizacaoPerfil { get; set; }

        [LogicalAttribute("itbc_aceita_ser_indicado_onde_encontrar")]
        public Boolean? AceitaSerIndicadoOndeEncontrar { get; set; }

        [LogicalAttribute("itbc_versao_termo_onde_encontrar")]
        public int? VersaoTermoOndeEncontrar { get; set; }

        [LogicalAttribute("itbc_cnpj_filiais")]
        public string CNPJFiliais { get; set; }

        [LogicalAttribute("itbc_marcas_enviadas")]
        public string MarcasEnviadas { get; set; }

        [LogicalAttribute("itbc_envio_sellout_estoque")]
        public int? EnvioSelloutEstoque { get; set; }

        [LogicalAttribute("itbc_obsrevacoes_servico")]
        public string ObservacoesServico { get; set; }

        [LogicalAttribute("itbc_possui_acesso_solar")]
        public Boolean? PossuiAcessoSolar { get; set; }

        [LogicalAttribute("itbc_comissao_projeto")]
        public Decimal? PercentualComissaoProjeto { get; set; }

        [LogicalAttribute("itbc_instalador")]
        public Boolean? Instalador { get; set; }
        #endregion
    }
}
