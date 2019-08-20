using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("pricelevel")]
    public class ListaPreco:DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ListaPreco() { }

        public ListaPreco(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ListaPreco(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }  
        #endregion

        #region Atributos
            [LogicalAttribute("pricelevelid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("begindate")]
            public DateTime? DataInicio { get; set; }

            [LogicalAttribute("description")]
            public String Descrição { get; set; }

            [LogicalAttribute("enddate")]
            public DateTime? DataTermino { get; set; }
        
            [LogicalAttribute("itbc_businessunitid")]
            public Lookup UnidadeNegocio  { get; set; }

            [LogicalAttribute("itbc_tipodalista")]
            public int? TipoLista { get; set; }

            [LogicalAttribute("name")]
            public string Nome { get; set; }

            [LogicalAttribute("statecode")]
            public int? Status { get; set; }

            [LogicalAttribute("transactioncurrencyid")]
            public Lookup Moeda { get; set; }

        #endregion
    }
}
