using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_itbc_prodestabelecimento")]
    public class ProdutoEstabelecimento : DomainBase
    {

        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public ProdutoEstabelecimento() { }

        public ProdutoEstabelecimento(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public ProdutoEstabelecimento(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_itbc_prodestabelecimentoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome {get; set;}

        [LogicalAttribute("itbc_estabelecimentoid")]
        public Lookup Estabelecimento { get; set; }

        [LogicalAttribute("itbc_productid")]
        public Lookup Produto { get; set; }

        [LogicalAttribute("statuscode")]
        public int? Status { get; set; }

        #endregion
    }
}