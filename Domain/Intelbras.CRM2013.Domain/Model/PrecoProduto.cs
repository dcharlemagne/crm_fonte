using System;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    public class PrecoProduto : DomainBase
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public PrecoProduto() { }

        public PrecoProduto(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public PrecoProduto(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos

        public Guid ContaId { get; set; }

        public Guid ProdutoId { get; set; }

        public string CodigoProduto { get; set; }

        public Guid? PoliticaComercialID { get; set; }

        public string Retorno { get; set; }

        public int Quantidade { get; set; }

        public int? QuantidadeFinal { get; set; }

        public string NomePoliticaComercial { get; set; }

        public decimal ValorProduto { get; set; }

        public decimal ValorBase { get; set; }

        public decimal? CustoProduto { get; set; }

        private bool CrossSellingDefault = false;

        public bool TipoCrossSelling { get { return CrossSellingDefault; } set { CrossSellingDefault = value; } }

        public string Moeda { get; set; }

        public Product Produto { get; set; }

        //Atributos nova politica comercial
        public int tipoPortofolio { get; set; }

        public int codEstabelecimento { get; set; }
        public string codUnidade { get; set; }
        public string codFamiliaComl { get; set; }
        public DateTime? DataValidade { get; set; }
        public bool? RebateAntecipado { get; set; }
        public decimal? PercentualRebateAntecipado { get; set; }
        public bool? CalcularRebate { get; set; }
        public bool? PrecoAlterado { get; set; }
        public decimal? ValorComDesconto { get; set; }
        public decimal? PercentualDesconto { get; set; }
        public decimal? PercentualPossivelDesconto { get; set; }

        #endregion
    }
}
