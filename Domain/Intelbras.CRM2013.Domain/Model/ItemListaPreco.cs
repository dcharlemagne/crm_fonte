using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("productpricelevel")]
    public class ItemListaPreco:DomainBase
    {
        #region Construtores

            private RepositoryService RepositoryService { get; set; }

        public ItemListaPreco() { }

        public ItemListaPreco(string organization, bool isOffline)
                : base(organization, isOffline)
            {
                RepositoryService = new RepositoryService(organization, isOffline);
            }

            public ItemListaPreco(string organization, bool isOffline, object provider)
                : base(organization, isOffline, provider)
            {
                RepositoryService = new RepositoryService(organization, isOffline, provider);
            }

        #endregion

        #region Atributos 
            [LogicalAttribute("productpricelevelid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("pricelevelid")]
            public Lookup ListaPrecos { get; set; }

            [LogicalAttribute("amount")]
            public Decimal? Valor { get; set; }    
            
            [LogicalAttribute("discounttypeid")]
            public Lookup ListaDesconto { get; set; }

            [LogicalAttribute("percentage")]
            public Decimal? Porcentual { get; set; }    

            [LogicalAttribute("pricingmethodcode")]
            public int? MetodoPrecificacao { get; set; }

            [LogicalAttribute("productid")]
            public Lookup ProdutoID { get; set; }    

            [LogicalAttribute("quantitysellingcode")]
            public int? OpcaoVendaParcial { get; set; }

            [LogicalAttribute("roundingoptionamount")]
            public Decimal? ValorArredondamento { get; set; }

            [LogicalAttribute("roundingoptioncode")]
            public int? OpcaoArredondamento { get; set; }    

            [LogicalAttribute("roundingpolicycode")]
            public int? PoliticaArredondamento { get; set; }

            [LogicalAttribute("transactioncurrencyid")]
            public Lookup Moeda { get; set; }

            [LogicalAttribute("uomid")]
            public Lookup Unidade { get; set; }    
        #endregion

    }
}
