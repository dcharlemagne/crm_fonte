using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_orcdocanaldetalhadoporproduto")]
    public class OrcamentodoCanalDetalhadoporProduto : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public OrcamentodoCanalDetalhadoporProduto(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public OrcamentodoCanalDetalhadoporProduto(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_orcdocanaldetalhadoporprodutoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_orcamentodocanalId")]
        public Lookup OrcamentodoCanal { get; set; }

        [LogicalAttribute("itbc_IntegradoPor")]
        public String IntegradoPor { get; set; }

        [LogicalAttribute("itbc_QtdeRealizada")]
        public Decimal QtdeRealizada { get; set; }

        [LogicalAttribute("itbc_OrcamentoRealizado")]
        public decimal? OrcamentoRealizado { get; set; }

        [LogicalAttribute("itbc_QtdePlanejada")]
        public Decimal? QtdePlanejada { get; set; }

        [LogicalAttribute("itbc_OrcamentoPlanejado")]
        public decimal? OrcamentoPlanejado { get; set; }

        [LogicalAttribute("itbc_UltAtualizacaoIntegracao")]
        public DateTime? UltAtualizacaoIntegracao { get; set; }

        [LogicalAttribute("itbc_Trimestre")]
        public int? Trimestre { get; set; }

        [LogicalAttribute("itbc_ProdutoId")]
        public Lookup Produto { get; set; }

        [LogicalAttribute("itbc_CanalId")]
        public Lookup Canal { get; set; }

        [LogicalAttribute("itbc_Mes")]
        public int? Mes { get; set; }

        [LogicalAttribute("itbc_Ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("itbc_OrcdoCanalporProdutoId")]
        public Lookup OrcamentodoCanalporProduto { get; set; }

        [LogicalAttribute("itbc_UsuarioIntegracao")]
        public String UsuarioIntegracao { get; set; }

        private Product _Produt = null;
        public Product Producto
        {
            get
            {
                if (_Produt == null && Produto.Id != null && Produto.Id != Guid.Empty)
                    _Produt = RepositoryService.Produto.Retrieve(Produto.Id);

                return _Produt;
            }
        }
        //[LogicalAttribute("itbc_orcdocanaldetalhadoporprodutoid")]
        //public Guid? ID { get; set; }

        //[LogicalAttribute("itbc_OrcamentoPlanejado")]
        //public decimal? OrcamentoPlanejado { get; set; }

        //[LogicalAttribute("itbc_Ano")]
        //public int? Ano { get; set; }

        //[LogicalAttribute("itbc_name")]
        //public String Nome { get; set; }

        //[LogicalAttribute("itbc_OrcamentoRealizado")]
        //public decimal? OrcamentoRealizado { get; set; }

        //[LogicalAttribute("itbc_QtdePlanejada")]
        //public Decimal? QtdePlanejada { get; set; }

        //[LogicalAttribute("itbc_UltAtualizacaoIntegracao")]
        //public DateTime? UltAtualizacaoIntegracao { get; set; }

        //[LogicalAttribute("itbc_Trimestre")]
        //public int? Trimestre { get; set; }

        //[LogicalAttribute("itbc_canalid")]
        //public Lookup Canal { get; set; }

        //[LogicalAttribute("itbc_Mes")]
        //public int? Mes { get; set; }

        //[LogicalAttribute("itbc_OrcdoCanalporProdutoId")]
        //public Lookup OrcamentodoCanalporProduto { get; set; }

        //[LogicalAttribute("itbc_QtdeRealizada")]
        //public Decimal? QtdeRealizada { get; set; }

        //[LogicalAttribute("itbc_IntegradoPor")]
        //public String IntegradoPor { get; set; }

        //[LogicalAttribute("itbc_ProdutoId")]
        //public Lookup Produto { get; set; }

        //[LogicalAttribute("itbc_UsuarioIntegracao")]
        //public String UsuarioIntegracao { get; set; }

        //[LogicalAttribute("itbc_orcamentodocanalid")]
        //public Lookup OrcamentodoCanal { get; set; }

        //[LogicalAttribute("itbc_unidadenegocioid")]
        //public Lookup UnidadeNegocio { get; set; }

        #endregion
    }
}

