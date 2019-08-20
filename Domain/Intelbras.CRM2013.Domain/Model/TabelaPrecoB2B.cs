using System;
using SDKore.Crm.Util;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_tabelaprecob2b")]
    public class TabelaPrecoB2B : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public TabelaPrecoB2B(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public TabelaPrecoB2B(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_tabelaprecob2bid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_tabelaprecoems")]
        public String CodigoTabelaPrecoEMS { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }
        
        [LogicalAttribute("itbc_datafinal")]
        public DateTime? DataFinal { get; set; }

        [LogicalAttribute("itbc_datainicial")]
        public DateTime? DataInicial { get; set; }

        [LogicalAttribute("itbc_codigomoeda")]
        public int? CodigoMoeda { get; set; }
        
        [LogicalAttribute("itbc_nomemoeda")]
        public String NomeMoeda { get; set; }
        #endregion
    }
}
