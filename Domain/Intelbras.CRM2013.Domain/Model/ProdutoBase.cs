using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    [LogicalEntity("product")]
    public class ProdutoBase : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ProdutoBase() { }

        public ProdutoBase(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ProdutoBase(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("productid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("currentcost")]
        public decimal? CustoAtual { get; set; }

        [LogicalAttribute("defaultuomid")]
        public Lookup UnidadePadrao { get; set; }

        [LogicalAttribute("defaultuomscheduleid")]
        public Lookup GrupoUnidades { get; set; }

        [LogicalAttribute("description")]
        public String Descricao { get; set; }

        [LogicalAttribute("itbc_businessunitid")]
        public Lookup UnidadeNegocio { get; set; }

        [LogicalAttribute("itbc_complemento")]
        public String Complemento { get; set; }

        [LogicalAttribute("itbc_acumulo_outroproduto")]
        public Boolean? FaturamentoOutroProduto { get; set; }

        [LogicalAttribute("itbc_considera_orc_metas")]
        public Boolean? ConsideraOrcamentoMeta { get; set; }

        [LogicalAttribute("itbc_qtde_mult_produto")]
        public Decimal? QuantidadeMultiplaProduto { get; set; }

        [LogicalAttribute("itbc_exigetreinamento")]
        public Boolean? ExigeTreinamento { get; set; }

        [LogicalAttribute("itbc_familia_material")]
        public Lookup FamiliaMaterial { get; set; }

        [LogicalAttribute("itbc_familiacomercial")]
        public Lookup FamiliaComercial { get; set; }

        [LogicalAttribute("itbc_familiadeprodid")]
        public Lookup FamiliaProduto { get; set; }

        [LogicalAttribute("itbc_grupodeestoque")]
        public Lookup GrupoEstoque { get; set; }

        [LogicalAttribute("itbc_origem")]
        public Lookup Origem { get; set; }

        [LogicalAttribute("itbc_rebateposvendaativadoid")]
        public Boolean? RebatePosVendaAtivado { get; set; }

        [LogicalAttribute("itbc_segmentoid")]
        public Lookup Segmento { get; set; }

        [LogicalAttribute("itbc_subfamiliadeproduto")]
        public Lookup SubfamiliaProduto { get; set; }

        [LogicalAttribute("itbc_tipo_produto")]
        public int? NaturezaProduto { get; set; }

        [LogicalAttribute("name")]
        public String Nome { get; set; }

        //[LogicalAttribute("price")]
        //public Decimal? PrecoLista { get; set; }

        [LogicalAttribute("pricelevelid")]
        public Lookup ListaPrecoPadrao { get; set; }

        [LogicalAttribute("productnumber")]
        public String Codigo { get; set; }

        [LogicalAttribute("productnumber")]
        public new String CodigoEms
        {
            get { return Codigo; }
            set { }
        }

        [LogicalAttribute("producttypecode")]
        public Int32? TipoProdutoid { get; set; }

        [LogicalAttribute("producturl")]
        public String Url { get; set; }

        [LogicalAttribute("quantitydecimal")]
        public Int32? QuantidadeDecimal { get; set; }

        [LogicalAttribute("quantityonhand")]
        public Decimal? QuantidadeDisponivel { get; set; }

        //[LogicalAttribute("standardcost")]
        //public Decimal? CustoPadrao { get; set; }

        [LogicalAttribute("itbc_showroom")]
        public bool? Showroom { get; set; }

        [LogicalAttribute("itbc_showroomrevenda")]
        public bool? ShowroomRevenda { get; set; }

        [LogicalAttribute("itbc_backupdistribuidor")]
        public bool? BackupDistribuidor { get; set; }

        [LogicalAttribute("itbc_backuprevenda")]
        public bool? BackupRevenda { get; set; }

        [LogicalAttribute("stockvolume")]
        public Decimal? VolumeEstoque { get; set; }

        [LogicalAttribute("stockweight")]
        public Decimal? PesoEstoque { get; set; }

        [LogicalAttribute("subjectid")]
        public Lookup Assunto { get; set; }

        [LogicalAttribute("suppliername")]
        public String NomeFornecedor { get; set; }

        [LogicalAttribute("transactioncurrencyid")]
        public Lookup Moeda { get; set; }

        [LogicalAttribute("vendorname")]
        public String Fornecedor { get; set; }

        [LogicalAttribute("vendorpartnumber")]
        public String NumeroPecaFabricante { get; set; }

        [LogicalAttribute("itbc_acaocrm")]
        public Boolean? IntegrarNoPlugin { get; set; }

        [LogicalAttribute("itbc_acumulo_outroproduto")]
        public Boolean? AcumulaOutroProduto { get; set; }

        [LogicalAttribute("itbc_dataultalteracaopvc")]
        public DateTime? DataUltAlteracaoPVC { get; set; }

        [LogicalAttribute("itbc_percentualdeipi")]
        public Decimal? PercentualIPI { get; set; }

        [LogicalAttribute("itbc_ncm")]
        public String NCM { get; set; }

        [LogicalAttribute("itbc_ean")]
        public String EAN { get; set; }

        [LogicalAttribute("itbc_bloquearcomercializacao")]
        public Boolean? BloquearComercializacao { get; set; }

        [LogicalAttribute("itbc_possuisubstituto")]
        public Boolean? PossuiSubstituto { get; set; }

        [LogicalAttribute("itbc_produtosubstitutoid")]
        public Lookup ProdutoSubstituto { get; set; }

        [LogicalAttribute("itbc_ekit")]
        public Boolean? EKit { get; set; }

        [LogicalAttribute("Itbc_temmensagem")]
        public Boolean TemMensagem { get; set; }

        [LogicalAttribute("Itbc_mensagem")]
        public String Mensagem { get; set; }

        [LogicalAttribute("itbc_podesercomercializadoforadokit")]
        public Boolean? ComercializadoForaKit { get; set; }

        [LogicalAttribute("itbc_politicadeposvenda")]
        public int? PoliticaPosVenda { get; set; }

        [LogicalAttribute("itbc_tempodegarantia")]
        public int? TempoGarantia { get; set; }

        [LogicalAttribute("itbc_permitiremsolbenef")]
        public Boolean? PermitirEmSolBenef { get; set; }

        public Int32 PortfolioTipo { get; set; }

        //CRM4
        private FamiliaComercial dadosFamiliaComercial = null;
        private decimal preco = 0, aliquotaIPI = 0, valorIPI = 0, aliquotaICMS = 0, valorICMS = 0;
        private int sequenciaEstrutura = 0;
        private int nivelEstrutura = 0;
        private int quantidadeNaEstrutura = 0;
        private bool permitidoVenda = true;
        private string localDeMontagem = "";
        private int garantiaEmDias = 0;
        private int quantidadeMinimaMultipla = 0;
        private DateTime? dataFabricacaoProduto = null;
        [LogicalAttribute("new_valor_mao_de_obra")]
        public decimal? ValorDaMaoDeObra { get; set; }
        

        public decimal ValorICMS
        {
            get { return valorICMS; }
            set { valorICMS = value; }
        }

        public decimal ValorIPI
        {
            get { return valorIPI; }
            set { valorIPI = value; }
        }

        public decimal AliquotaICMS
        {
            get { return aliquotaICMS; }
            set { aliquotaICMS = value; }
        }

        [LogicalAttribute("new_intervencao_tecnica")]
        public bool? IntervencaoTecnica { get; set; }

        public int QuantidadeMinimaMultipla
        {
            get { return quantidadeMinimaMultipla; }
            set { quantidadeMinimaMultipla = value; }
        }

        public int QuantidadeNaEstrutura
        {
            get { return quantidadeNaEstrutura; }
            set { quantidadeNaEstrutura = value; }
        }

        public int GarantiaEmDias
        {
            get { return garantiaEmDias; }
            set { garantiaEmDias = value; }
        }

        public string LocalDeMontagem
        {
            get { return localDeMontagem; }
            set { localDeMontagem = value; }
        }

        public bool PermitidoVenda
        {
            get { return permitidoVenda; }
            set { permitidoVenda = value; }
        }

        public int NivelEstrutura
        {
            get { return nivelEstrutura; }
            set { nivelEstrutura = value; }
        }

        public int SequenciaEstrutura
        {
            get { return sequenciaEstrutura; }
            set { sequenciaEstrutura = value; }
        }

        public DateTime? DataFabricacaoProduto
        {
            get { return dataFabricacaoProduto; }
            set { dataFabricacaoProduto = value; }
        }

        public decimal Preco
        {
            get { return preco; }
            set { preco = value; }
        }

        public decimal AliquotaIPI
        {
            get { return aliquotaIPI; }
            set { aliquotaIPI = value; }
        }
    
        private bool permiteOS = false;
        public bool PermiteOS
        {
            get { return permiteOS; }
            set { permiteOS = value; }
        }

        [LogicalAttribute("itbc_depositopadrao")]
        public String DepositoPadrao { get; set; }

        [LogicalAttribute("itbc_codigotipodespesa")]
        public int? CodigoTipoDespesa { get; set; }

        [LogicalAttribute("itbc_destaquencm")]
        public int? DestaqueNCM { get; set; }

        [LogicalAttribute("itbc_nve")]
        public String NVE { get; set; }

        [LogicalAttribute("itbc_codigounidadefamilia")]
        public String CodigoUnidadeFamilia { get; set; }
        #endregion
    }
}
