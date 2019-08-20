using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_itenstabelaprecob2b")]
    public class ItemTabelaPrecoB2B : DomainBase
    {
        #region Construtores

            private RepositoryService RepositoryService { get; set; }
    
            public ItemTabelaPrecoB2B(string organization, bool isOffline)
                : base(organization, isOffline)
            {
                RepositoryService = new RepositoryService(organization, isOffline);
            }

            public ItemTabelaPrecoB2B(string organization, bool isOffline, object provider)
                : base(organization, isOffline, provider)
            {
                RepositoryService = new RepositoryService(organization, isOffline, provider);
            }

        #endregion

        #region Atributos 
            [LogicalAttribute("itbc_itenstabelaprecob2bid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("itbc_tabelapreco")]
            public Lookup TabelaPreco { get; set; }

            [LogicalAttribute("itbc_codigoproduto")]
            public Lookup Produto { get; set; }

            [LogicalAttribute("itbc_codigoitempreco")]
            public String CodigoItemPreco { get; set; }
        
            [LogicalAttribute("itbc_valorpma")]
            public Decimal? ValorPMA { get; set; }

            [LogicalAttribute("itbc_valorpmd")]
            public Decimal? ValorPMD { get; set; }

            [LogicalAttribute("itbc_precofob")]
            public Decimal? PrecoFOB { get; set; }    

            [LogicalAttribute("itbc_precominimocif")]
            public Decimal? PrecoMinimoCIF { get; set; }

            [LogicalAttribute("itbc_precominimofob")]
            public Decimal? PrecoMinimoFOB { get; set; }

            [LogicalAttribute("itbc_precounico")]
            public Decimal? PrecoUnico { get; set; }
  
            [LogicalAttribute("itbc_precovenda")]
            public Decimal? PrecoVenda { get; set; }

            [LogicalAttribute("itbc_quantidademinima")]
            public Decimal? QuantidadeMinima { get; set; }

        #endregion

    }
}
