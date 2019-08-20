using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_portfolio")]
    public class Portfolio : DomainBase
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Portfolio() { }

        public Portfolio(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public Portfolio(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_portfolioid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }

        [LogicalAttribute("itbc_businessunitid")]
        public Lookup UnidadeNegocio { get; set; }

        [LogicalAttribute("itbc_tipoid")]
        public int? Tipo { get; set; }

        [LogicalAttribute("itbc_classificacaoid")]
        public Lookup Classificacao { get; set; }

        [LogicalAttribute("statuscode")]
        public int? Status { get; set; }

        #endregion
    }
}
