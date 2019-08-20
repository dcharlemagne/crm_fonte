using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_tabeladepreco")]
    public class TabelaPreco : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public TabelaPreco() { }

        public TabelaPreco(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public TabelaPreco(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_tabeladeprecoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_codigo_tabela")]
        public String Codigo { get; set; }

        [LogicalAttribute("itbc_datafinal")]
        public DateTime? DataFinal { get; set; }

        [LogicalAttribute("itbc_datainicial")]
        public DateTime? DataInicial { get; set; }

        [LogicalAttribute("itbc_moeda")]
        public Decimal? Moeda { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("statecode")]
        public int? Status { get; set; }
        #endregion
    }
}
