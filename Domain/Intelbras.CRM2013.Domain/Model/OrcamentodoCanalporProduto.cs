using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_orcdocanalporproduto")]
    public class OrcamentodoCanalporProduto : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public OrcamentodoCanalporProduto(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public OrcamentodoCanalporProduto(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_orcdocanalporprodutoid")]
        public Guid? ID { get; set; }
        
        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }
        
        [LogicalAttribute("itbc_QtdeRealizada")]
        public Decimal QtdeRealizada { get; set; }
        
        [LogicalAttribute("itbc_QtdePlanejada")]
        public Decimal QtdePlanejada { get; set; }
        
        [LogicalAttribute("itbc_Trimestre")]
        public int? Trimestre { get; set; }
        
        [LogicalAttribute("itbc_OrcamentoPlanejado")]
        public decimal? OrcamentoPlanejado { get; set; }
        
        [LogicalAttribute("itbc_UltAtualizacaoIntegracao")]
        public DateTime? UltAtualizacaoIntegracao { get; set; }
        
        [LogicalAttribute("itbc_ProdutoId")]
        public Lookup Produto { get; set; }
        
        [LogicalAttribute("itbc_OrcamentoRealizado")]
        public decimal? OrcamentoRealizado { get; set; }
        
        [LogicalAttribute("itbc_CanalId")]
        public Lookup Canal { get; set; }
        
        [LogicalAttribute("itbc_Ano")]
        public int? Ano { get; set; }
        
        [LogicalAttribute("itbc_IntegradoPor")]
        public String IntegradoPor { get; set; }
        
        [LogicalAttribute("itbc_UsuarioIntegracao")]
        public String UsuarioIntegracao { get; set; }
        
        [LogicalAttribute("itbc_OrcamentodoCanalporSubFamilia")]
        public Lookup OrcamentodoCanalporSubFamilia { get; set; }
        
        public Conta _conta = null;
        public Conta Conta
        {
            get
            {
                if (_conta == null && Canal.Id != null && Canal.Id != Guid.Empty)
                    _conta = RepositoryService.Conta.Retrieve(Canal.Id);

                return _conta;
            }
        }

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

        //[LogicalAttribute("itbc_orcdocanalporprodutoid")]
        //public Guid? ID { get; set; }
        
        //[LogicalAttribute("itbc_name")]
        //public String Nome { get; set; }
        
        //[LogicalAttribute("itbc_QtdeRealizada")]
        //public Decimal QtdeRealizada { get; set; }
        
        //[LogicalAttribute("itbc_OrcamentoRealizado")]
        //public decimal? OrcamentoRealizado { get; set; }
        
        //[LogicalAttribute("itbc_QtdePlanejada")]
        //public Decimal QtdePlanejada { get; set; }
        
        //[LogicalAttribute("itbc_UltAtualizacaoIntegracao")]
        //public DateTime? UltAtualizacaoIntegracao { get; set; }
        
        //[LogicalAttribute("itbc_Trimestre")]
        //public int? Trimestre { get; set; }
        
        //[LogicalAttribute("itbc_ProdutoId")]
        //public Lookup Produto { get; set; }
        
        //[LogicalAttribute("itbc_CanalId")]
        //public Lookup Canal { get; set; }

        
        
        //[LogicalAttribute("itbc_OrcamentoPlanejado")]
        //public decimal? OrcamentoPlanejado { get; set; }
                
        //[LogicalAttribute("itbc_Ano")]
        //public int? Ano { get; set; }
        
        //[LogicalAttribute("itbc_IntegradoPor")]
        //public String IntegradoPor { get; set; }
        
        //[LogicalAttribute("itbc_UsuarioIntegracao")]
        //public String UsuarioIntegracao { get; set; }
                
        //[LogicalAttribute("itbc_OrcamentodoCanalporSubFamilia")]
        //public Lookup OrcamentodoCanalporSubFamilia { get; set; }

        

        //[LogicalAttribute("itbc_unidadenegocioid")]
        //public Lookup UnidadeNegocio { get; set; }
        #endregion
    }
}

