using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_produtosdalistapsdid")]
    public class ProdutoListaPSDPPPSCF : DomainBase
    {
        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public ProdutoListaPSDPPPSCF() { }

        public ProdutoListaPSDPPPSCF(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public ProdutoListaPSDPPPSCF(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_produtosdalistapsdidid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_productid")]
        public Lookup Produto { get; set; }

        [LogicalAttribute("itbc_psdid")]
        public Lookup PSD { get; set; }

        [LogicalAttribute("itbc_psdo")]
        public Boolean? PSDControlado { get; set; }
        
        [LogicalAttribute("itbc_controlapsd")]
        public int? ControlaPSD { get; set; }

        [LogicalAttribute("itbc_valor")]
        public Decimal? ValorPSD { get; set; }

        [LogicalAttribute("itbc_valorpp")]
        public Decimal? ValorPP { get; set; }

        [LogicalAttribute("itbc_valorpscf")]
        public Decimal? ValorPSCF { get; set; }

        [LogicalAttribute("transactioncurrencyid")]
        public Lookup Moeda { get; set; }
        #endregion
    }
}
