using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_solicitacaodebeneficio")]
    public class SolicitacaoBeneficio : DomainBase
    {
        #region Construtores

        public SolicitacaoBeneficio() { }

        public SolicitacaoBeneficio(string organization, bool isOffline)
            : base(organization, isOffline)
        {
        }

        public SolicitacaoBeneficio(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
        }

        #endregion

        #region Atributos

        [LogicalAttribute("itbc_solicitacaodebeneficioid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_benefdocanal")]
        public Lookup BeneficioCanal { get; set; }

        [LogicalAttribute("itbc_karepresentanteresponsvel")]
        public Lookup KaRepresentante { get; set; }

        [LogicalAttribute("itbc_beneficiodoprograma")]
        public Lookup BeneficioPrograma { get; set; }

        [LogicalAttribute("itbc_condicaopagamentoid")]
        public Lookup CondicaoPagamento { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }

        [LogicalAttribute("itbc_accountid")]
        public Lookup Canal { get; set; }

        [LogicalAttribute("itbc_solicitacaoprincipalid")]
        public Lookup SolicitacaoBeneficioPrincipal { get; set; }

        [LogicalAttribute("itbc_status")]
        public int? StatusSolicitacao { get; set; }

        [LogicalAttribute("itbc_tipodepriceprotection")]
        public int? TipoPriceProtection { get; set; }

        [LogicalAttribute("statuscode")]
        public int? Status { get; set; }

        [LogicalAttribute("statecode")]
        public int? State { get; set; }

        [LogicalAttribute("createdon")]
        public DateTime? DataCriacao { get; set; }

        [LogicalAttribute("itbc_businessunitid")]
        public Lookup UnidadedeNegocio { get; set; }

        [LogicalAttribute("itbc_valoraprovado")]
        public decimal? ValorAprovado { get; set; }

        [LogicalAttribute("itbc_valordaacao")]
        public decimal? ValorAcao { get; set; }

        [LogicalAttribute("itbc_situacao_irregular")]
        public String SituacaoIrregular { get; set; }

        [LogicalAttribute("itbc_descricao")]
        public String Descricao { get; set; }

        [LogicalAttribute("itbc_solicitacao_irregularidades")]
        public Boolean? SituacaoIrregularidades { get; set; }

        [LogicalAttribute("itbc_acaosubsidiadavmcid")]
        public Lookup AcaoSubsidiadaVmc { get; set; }

        [LogicalAttribute("itbc_valorsolicitado")]
        public decimal? ValorSolicitado { get; set; }

        [LogicalAttribute("itbc_formapagamentoid")]
        public Lookup FormaPagamento { get; set; }

        [LogicalAttribute("itbc_dt_inicio_acao")]
        public DateTime? DataIniAcao { get; set; }

        [LogicalAttribute("itbc_dataprevistaderetorno")]
        public DateTime? DataFimAcao { get; set; }
        
        [LogicalAttribute("itbc_dataaprovacao")]
        public DateTime? DataAprovacao { get; set; }

        [LogicalAttribute("itbc_valorpago")]
        public decimal? ValorPago { get; set; }

        [LogicalAttribute("itbc_alteradaparastockrotation")]
        public Boolean? AlteradaParaStockRotation { get; set; }

        [LogicalAttribute("itbc_tipodesolicitacaoid")]
        public Lookup TipoSolicitacao { get; set; }

        [LogicalAttribute("itbc_acaocrm")]
        public bool? IntegrarNoPlugin { get; set; }

        [LogicalAttribute("itbc_assistadmvendasid")]
        public Lookup Assistente { get; set; }

        [LogicalAttribute("itbc_supervisorid")]
        public Lookup Supervisor { get; set; }

        [LogicalAttribute("itbc_filialid")]
        public Lookup Filial { get; set; }

        [LogicalAttribute("itbc_statusdopagamento")]
        public int? StatusPagamento { get; set; }

        [LogicalAttribute("itbc_ajustesaldo")]
        public Boolean? AjusteSaldo { get; set; }

        [LogicalAttribute("itbc_valoraabater")]
        public decimal? ValorAbater { get; set; }

        [LogicalAttribute("itbc_ultatualizacaointegracao")]
        public DateTime? IntegradoEm { get; set; }

        [LogicalAttribute("itbc_IntegradoFrom")]
        public string IntegradoDe { get; set; }

        [LogicalAttribute("itbc_usuariointegracao")]
        public string UsuarioIntegracao { get; set; }

        [LogicalAttribute("itbc_integradopor")]
        public string IntegradoPor { get; set; }

        [LogicalAttribute("itbc_valorcancelado")]
        public decimal? ValorCancelado { get; set; }

        [LogicalAttribute("itbc_datavalidade")]
        public DateTime? DataValidade { get; set; }

        [LogicalAttribute("itbc_descartarverba")]
        public Boolean? DescartarVerba { get; set; }

        [LogicalAttribute("itbc_trimestrecompetencia")]
        public string TrimestreCompetencia { get; set; }

        [LogicalAttribute("itbc_formacancelamento")]
        public int? FormaCancelamento { get; set; }

        [LogicalAttribute("itbc_criada_apos_data_limite")]
        public bool? CriadaAposDataLimite { get; set; }

        [LogicalAttribute("itbc_statuscalculopriceprotection")]
        public int? StatusCalculoPriceProtection { get; set; }

        [LogicalAttribute("itbc_resultadoprevisto")]
        public decimal? ResultadoPrevisto { get; set; }

        [LogicalAttribute("itbc_resultadoalcancado")]
        public decimal? ResultadoAlcancado { get; set; }
        #endregion
    }
}