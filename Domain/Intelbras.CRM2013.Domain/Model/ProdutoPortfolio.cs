using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_proddoport")]
    public class ProdutoPortfolio : DomainBase
    {

        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public ProdutoPortfolio() { }

        public ProdutoPortfolio(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public ProdutoPortfolio(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_proddoportid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome {get; set;}

        [LogicalAttribute("itbc_portfolioid")]
        public Lookup Portfolio { get; set; }

        [LogicalAttribute("itbc_exige_treinamento")]
        public bool ExigeTreinamento { get; set; }

        [LogicalAttribute("itbc_prod_compartilhado_unid")]
        public int? prod_compartilhado { get; set; }

        [LogicalAttribute("itbc_productid")]
        public Lookup Produto { get; set; }

        private Product _Product = null;
        public Product Product
        {
            get
            {
                if (_Product == null && Produto.Id != Guid.Empty)
                    _Product = RepositoryService.Produto.Retrieve(Produto.Id);

                return _Product;
            }
        }

        [LogicalAttribute("statuscode")]
        public int? Status { get; set; }

        [LogicalAttribute("statecode")]
        public int? StateCode { get; set; }

        #region Model Utilizado para produtosPortfolio
        public string CodigoProduto { get; set; }

        public string PortfolioNome { get; set; }

        public Int32 PortfolioTipo { get; set; }

        public string ProdutoNome { get; set; }

        public bool ProdutoShowRoom { get; set; }

        public bool ProdutoBloqueado { get; set; }

        public decimal? CustoAtual { get; set; }

        public decimal? PrecoSemPoliticaComercial { get; set; }

        public decimal? PrecoPoliticaComercial { get; set; }

        public string NomePoliticaComercial { get; set; }

        public Guid? PoliticaComercialID { get; set; }

        public Guid? CanalId { get; set; }

        #endregion

        #endregion
    }
}
