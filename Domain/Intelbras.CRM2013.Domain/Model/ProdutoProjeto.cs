using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_produto_do_cliente_potencial")]
    public class ProdutoProjeto : DomainBase
    {

        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public ProdutoProjeto(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public ProdutoProjeto(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_produto_do_cliente_potencialid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }

        [LogicalAttribute("itbc_valor_unitario")]
        public Decimal? ValorUnitario { get; set; }

        [LogicalAttribute("itbc_quantidade")]
        public decimal? Quantidade { get; set; }

        [LogicalAttribute("itbc_valor_total")]
        public Decimal? ValorTotal { get; set; }

        [LogicalAttribute("itbc_produto")]
        public Lookup Produto { get; set; }

        [LogicalAttribute("itbc_cliente_potencial")]
        public Lookup ClientePotencial { get; set; }

        #endregion
    }
}
