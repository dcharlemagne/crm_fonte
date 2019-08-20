using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_histricodecomprasdocanal")]
    public class HistoricoComprasCanalProdutoMes : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public HistoricoComprasCanalProdutoMes() { }

        public HistoricoComprasCanalProdutoMes(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public HistoricoComprasCanalProdutoMes(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion


        #region Atributos
        [LogicalAttribute("itbc_histricodecomprasdocanalid")]
        public Guid? ID { get; set; }
    
        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_valor")]
        public decimal? Valor { get; set; }

        [LogicalAttribute("itbc_ano")]
        public Int32? Ano { get; set; }

        [LogicalAttribute("itbc_trimestre")]
        public int? Trimestre { get; set; }

        [LogicalAttribute("itbc_mes")]
        public int? Mes { get; set; }

        [LogicalAttribute("itbc_account")]
        public Lookup Canal { get; set; }

        [LogicalAttribute("itbc_product")]
        public Lookup Produto { get; set; }

        [LogicalAttribute("itbc_historicodecomprasdocanalid")]
        public Lookup HistoricoCanal { get; set; }

        [LogicalAttribute("itbc_quantidade")]
        public decimal? Quantidade { get; set; }

        #endregion
    }
}
