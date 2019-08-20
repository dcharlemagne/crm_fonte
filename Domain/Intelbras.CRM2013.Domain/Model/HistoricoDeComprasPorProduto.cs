using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_historicodecomprasporproduto")]
    public class HistoricoDeComprasPorProduto : DomainBase
    {
         #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public HistoricoDeComprasPorProduto() { }

        public HistoricoDeComprasPorProduto(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public HistoricoDeComprasPorProduto(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("itbc_historicodecomprasporprodutoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_ano")]
        public Int32 ano { get; set; }  

        [LogicalAttribute("itbc_trimestre")]
        public Int32 trimestre { get; set; }  

        [LogicalAttribute("itbc_produtoid")]
        public Lookup produto { get; set; }

        [LogicalAttribute("itbc_quantidade")]
        public decimal? quantidade { get; set; }    

        [LogicalAttribute("itbc_valor")]
        public decimal? Valor { get; set; }

        [LogicalAttribute("itbc_historicodecomprasporsubfamiliaid")]
        public Lookup SubFamilia { get; set; }


        #endregion
    }
    

}
