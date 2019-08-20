using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_linha_unidade_negocio")]
    public class LinhaComercial : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public LinhaComercial() { }

        public LinhaComercial(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }


        public LinhaComercial(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }


        #endregion

        #region Atributos
        
        
        [LogicalAttribute("itbc_canal_vendaid")]
        public Lookup CanalDeVendaId { get; set; }
        private CanaldeVenda canalDeVenda = null;
        public CanaldeVenda CanalDeVenda
        {
            get
            {
                if (canalDeVenda == null && CanalDeVendaId != null)
                    canalDeVenda = RepositoryService.CanaldeVenda.Retrieve(CanalDeVendaId.Id);
                return canalDeVenda;
            }
            set { canalDeVenda = value; }
        }
        [LogicalAttribute("new_percentual_minimo_pedido")]
        public Double? PercentualMinimoParaPedido { get; set; }
        [LogicalAttribute("new_unidade_negocioid")]
        public Lookup UnidadeDeNegocioId { get; set; }
        private UnidadeNegocio unidadeDeNegocio = null;
        public UnidadeNegocio UnidadeDeNegocio
        {
            get
            {
                if (UnidadeDeNegocioId != null && unidadeDeNegocio == null)
                    unidadeDeNegocio = RepositoryService.UnidadeNegocio.Retrieve(UnidadeDeNegocioId.Id);

                return unidadeDeNegocio;
            }
            set { unidadeDeNegocio = value; }
        }
        [LogicalAttribute("itbc_estabelecimentoid")]
        public Lookup EstabelecimentoId { get; set; }
        private Estabelecimento estabelecimento = null;
        public Estabelecimento Estabelecimento
        {
            get
            {
                if (estabelecimento == null && EstabelecimentoId != null && this.Id != Guid.Empty)
                    estabelecimento = RepositoryService.Estabelecimento.Retrieve(EstabelecimentoId.Id);

                return estabelecimento;
            }
            set { estabelecimento = value; }
        }
        [LogicalAttribute("new_codigo_atendente")]
        public int? CodigoAtendente { get; set; }
        [LogicalAttribute("new_dias_reincidencia")]
        public int? NumeroDeDiasParaReincidencia { get; set; }
        [LogicalAttribute("new_numero_itens")]
        public int? NumeroDeItensParaReincidencia { get; set; }
        [LogicalAttribute("new_nota_fiscal")]
        public bool? ObrigatorioEnviarNotaFiscal { get; set; }
        [LogicalAttribute("itbc_garantia_adicional")]
        public int? GarantiaAdicional { get; set; }
        [LogicalAttribute("new_name")]
        public String Nome { get; set; }
        #endregion
    }
}
