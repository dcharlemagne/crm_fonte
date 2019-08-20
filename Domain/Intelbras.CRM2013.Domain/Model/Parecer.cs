using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_parecer")]
    public class Parecer : DomainBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public Parecer() { }

        public Parecer(String organization, bool isOffline):base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public Parecer(String organization, bool isOffline, object provider):base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_parecerid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_IntegradoPor")]
        public String IntegradoPor { get; set; }

        [LogicalAttribute("itbc_dispoe_suportetecnico")]
        public int? DispoedeSuporteTecnico { get; set; }

        [LogicalAttribute("itbc_distribuidores")]
        public String Distribuidores { get; set; }

        [LogicalAttribute("itbc_ContatoId")]
        public Lookup KeyAccountouRepresentante { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_aprov_redir")]
        public int? AprovadopeloREDIR { get; set; }

        [LogicalAttribute("itbc_vendas_clientesfinais")]
        public int? AtuacomVendaparaClientesFinais { get; set; }

        [LogicalAttribute("itbc_compet_tec_comerc")]
        public int? DispoeCompetenciaTecnicaeComercial { get; set; }

        [LogicalAttribute("itbc_envio_sellout_estoq")]
        public int? AceitouEnviarInformacoesdeSellouteEstoque { get; set; }

        [LogicalAttribute("itbc_capital_proprio")]
        public decimal? CapitalProprio { get; set; }

        [LogicalAttribute("itbc_fundamentacao_distrib_aprovado")]
        public string FundamentacaoDistribuidorAprovado { get; set; }

        [LogicalAttribute("itbc_capitalproprio_porcentagem")]
        public Decimal CapitalProprioPct { get; set; }

        [LogicalAttribute("itbc_VolumeTotalAnual")]
        public decimal? VolumeTotalAnual { get; set; }

        [LogicalAttribute("itbc_Capital_teceiros")]
        public decimal? CapitaldeTerceiros { get; set; }

        [LogicalAttribute("itbc_EstadoId")]
        public Lookup Estado { get; set; }

        [LogicalAttribute("itbc_impactos_abertura")]
        public string QuaisosImpactosdeAberturadoDistribuidor { get; set; }

        [LogicalAttribute("itbc_cond_padraominimo")]
        public int? Condicoesatendempadraominimo { get; set; }

        [LogicalAttribute("itbc_demanda_linhadeproduto")]
        public string HademandaporLinhasdeProdutosnosMunicipios { get; set; }

        [LogicalAttribute("itbc_sistema_admin_financ")]
        public int? DispoedeSistemadeGestaoAdmeFinanceira { get; set; }

        [LogicalAttribute("itbc_observ_gerentenacional")]
        public string ObservacoesdoGerenteNacional { get; set; }

        [LogicalAttribute("itbc_envio_docs")]
        public int? Envioudocumentacaocompleta { get; set; }

        [LogicalAttribute("itbc_defende_abert_distr")]
        public string PorqueDefendeAberturadesseDistribuidor { get; set; }

        [LogicalAttribute("itbc_parecer_tarefaId")]
        public Lookup Tarefa { get; set; }

        [LogicalAttribute("itbc_capitalterceiros_porcentagem")]
        public Decimal CapitaldeTerceirosPct { get; set; }

        [LogicalAttribute("itbc_fundamentacao")]
        public string Fundamentacao { get; set; }

        [LogicalAttribute("itbc_potencial_regiao")]
        public decimal? PotencialdaRegiao { get; set; }

        [LogicalAttribute("itbc_exclus_prod_intelbras")]
        public int? IraatuarsomentecomprodutosIntelbras { get; set; }

        [LogicalAttribute("itbc_parecer_keyaccount_repres")]
        public int? ParecerKeyAccountRepresentante { get; set; }

        [LogicalAttribute("itbc_aprov_comite")]
        public int? AprovadoPeloComite { get; set; }

        [LogicalAttribute("itbc_UltAtualizacaoIntegracao")]
        public DateTime? UltAtualizacaoIntegracao { get; set; }

        [LogicalAttribute("itbc_faturamento_regiao")]
        public decimal? FaturamentoDiretoparaaRegiao { get; set; }

        [LogicalAttribute("itbc_fichacadastral_distribuidor")]
        public int? FichadeAvaliacaodoDistribuidor { get; set; }

        [LogicalAttribute("itbc_motivos_abertura_distrib")]
        public string MotivosprincipaisdeaberturadoDistribuidor { get; set; }

        [LogicalAttribute("itbc_novas_praticas")]
        public int? DispostoaAtuarDentrodasNovasPraticas { get; set; }

        [LogicalAttribute("itbc_tipodoparecerglobal")]
        public int? TipodoParecer { get; set; }

        //Nao é usada no crm
        public string TipoDoParecerNome { get; set; }

        [LogicalAttribute("itbc_previsao_linhadecorte")]
        public string PrevisaoLinhadeCorteMinima { get; set; }

        [LogicalAttribute("itbc_parecer_setor_financ")]
        public int? ParecerSetorFinanceiro { get; set; }

        [LogicalAttribute("itbc_conflitos_distrib")]
        public string TeremosConflitoscomosDistribuidores { get; set; }

        [LogicalAttribute("itbc_UsuarioIntegracao")]
        public String UsuarioIntegracao { get; set; }

        [LogicalAttribute("itbc_observ_keyaccount_repres")]
        public string ObservacoesKeyAccountRepres { get; set; }

        [LogicalAttribute("itbc_ContaId")]
        public Lookup Canal { get; set; }

        [LogicalAttribute("itbc_limite_creditoliberado")]
        public decimal? QualLimitedeCreditoLiberado { get; set; }

        [LogicalAttribute("itbc_parecer_gerentenacional")]
        public int? ParecerdoGerenteNacionaldeVendas { get; set; }

        [LogicalAttribute("itbc_daRegio")]
        public Decimal PorcentagemFaturamento { get; set; }

        [LogicalAttribute("itbc_distribuidor_aprovado")]
        public int? DistribuidorAprovado { get; set; }

        [LogicalAttribute("itbc_atua_sem_nf")]
        public int? AtuacomVendasemNotaFiscal { get; set; }

        [LogicalAttribute("itbc_observ_setorfinanceiro")]
        public String ObservacoesSetorFinanceiro { get; set; }

        [LogicalAttribute("itbc_distribuidor_adequado")]
        public int? DistribuidorAdequado { get; set; }

        [LogicalAttribute("itbc_potencial_porcentagem")]
        public Decimal PotencialPorcentagem { get; set; }

        [LogicalAttribute("itbc_exper_distribuicao")]
        public int? PossuiExperienciade5anosemDistribuicao { get; set; }

        [LogicalAttribute("itbc_faturamento_porcentagem")]
        public Decimal FaturamentoPorcentagemRegiao { get; set; }

        [LogicalAttribute("statecode")]
        public Int32? Status { get; set; }

        /// <summary>
        /// Não tem na model do crm
        /// </summary>
        public string StatusName { get; set; }

        #endregion
    }
 }
