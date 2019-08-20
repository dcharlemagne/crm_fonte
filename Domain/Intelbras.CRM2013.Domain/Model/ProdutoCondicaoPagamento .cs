using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_produtodacondiodepagamento")]
    public class ProdutoCondicaoPagamento : DomainBase
    {

        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public ProdutoCondicaoPagamento() { }

        public ProdutoCondicaoPagamento(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public ProdutoCondicaoPagamento(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_produtodacondiodepagamentoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome {get; set;}

        [LogicalAttribute("itbc_condiodepagamento")]
        public Lookup CondicaoPagamento { get; set; }

        [LogicalAttribute("itbc_produto")]
        public Lookup Produto { get; set; }

        [LogicalAttribute("statecode")]
        public int? Status { get; set; }

        #endregion
    }
}