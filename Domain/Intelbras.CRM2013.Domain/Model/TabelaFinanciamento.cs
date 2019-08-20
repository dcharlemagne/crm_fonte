using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_tabeladefinanciamento")]
    public class TabelaFinanciamento : IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public TabelaFinanciamento() { }

        public TabelaFinanciamento(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public TabelaFinanciamento(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_tabeladefinanciamentoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_finalvalidade")]
        public DateTime? DataFinalValidade { get; set; }

        [LogicalAttribute("itbc_iniciovalidade")]
        public DateTime? DataInicioValidade { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("statecode")]
        public int? State { get; set; }
        #endregion
    }
}
