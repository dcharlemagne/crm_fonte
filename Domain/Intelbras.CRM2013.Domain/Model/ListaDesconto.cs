using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("discounttype")]
    public class ListaDesconto:IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ListaDesconto() { }

        public ListaDesconto(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ListaDesconto(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }  
        #endregion

        #region Atributos

            [LogicalAttribute("discounttypeid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("description")]
            public String Descrição { get; set; }

            [LogicalAttribute("name")]
            public string Nome { get; set; }

            [LogicalAttribute("statecode")]
            public int? Status { get; set; }

            [LogicalAttribute("transactioncurrencyid")]
            public Lookup Moeda { get; set; }
        
        #endregion
    }
}
