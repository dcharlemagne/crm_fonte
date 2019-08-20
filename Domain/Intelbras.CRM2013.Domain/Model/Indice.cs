using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_indice")]
    public class Indice:IntegracaoBase
    {
        #region Construtores

            private RepositoryService RepositoryService { get; set; }

        public Indice() { }

        public Indice(string organization, bool isOffline)
                : base(organization, isOffline)
            {
                RepositoryService = new RepositoryService(organization, isOffline);
            }

            public Indice(string organization, bool isOffline, object provider)
                : base(organization, isOffline, provider)
            {
                RepositoryService = new RepositoryService(organization, isOffline, provider);
            }

        #endregion

        #region Atributos
            [LogicalAttribute("itbc_indiceid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("itbc_chave_integracao")]
            public String ChaveIntegracao { get; set; }

            [LogicalAttribute("itbc_Dia_indice")]
            public Int32? DiaIndice { get; set; }

            [LogicalAttribute("itbc_indice")]
            public Decimal? Indiceid { get; set; }

            [LogicalAttribute("itbc_name")]
            public String Nome { get; set; }

            [LogicalAttribute("itbc_tabeladefinanciamento")]
            public Lookup TabelaFinanciamento { get; set; }

            [LogicalAttribute("StateCode")]
            public int? Status { get; set; }
        #endregion
    }
}
