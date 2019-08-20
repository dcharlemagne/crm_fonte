using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_histdecomprasdetdoproduto")]
    public class HistoricoDeComprasPorProdutoMes : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public HistoricoDeComprasPorProdutoMes() { }

        public HistoricoDeComprasPorProdutoMes(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public HistoricoDeComprasPorProdutoMes(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        [LogicalAttribute("itbc_histdecomprasdetdoprodutoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_ano")]
        public Int32 ano { get; set; }

        [LogicalAttribute("itbc_trimestre")]
        public Int32 trimestre { get; set; }

        [LogicalAttribute("itbc_mes")]
        public Int32 mes { get; set; }

        [LogicalAttribute("itbc_produtoid")]
        public Lookup produto { get; set; }

        //[LogicalAttribute("itbc_account")]
        //public Lookup canal { get; set; }

        [LogicalAttribute("itbc_quantidade")]
        public decimal? quantidade { get; set; }

        [LogicalAttribute("itbc_valor")]
        public decimal? Valor { get; set; }

        [LogicalAttribute("itbc_historicodecomprasdoprodutoid")]
        public Lookup HistoricoProduto { get; set; }
    }
}
