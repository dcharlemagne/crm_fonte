using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("transactioncurrency")]
    public class Moeda : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Moeda() { }

        public Moeda(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Moeda(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }


        #endregion

        #region Atributos
        [LogicalAttribute("transactioncurrencyid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("currencyname")]
        public String NomeMoeda { get; set; }

        [LogicalAttribute("currencyprecision")]
        public int? Precisão { get; set; }

        [LogicalAttribute("currencysymbol")]
        public String Simbolo { get; set; }

        [LogicalAttribute("exchangerate")]
        public Decimal? TaxaCambio { get; set; }

        [LogicalAttribute("isocurrencycode")]
        public String CodigoMoeda { get; set; }
        #endregion
    }
}
