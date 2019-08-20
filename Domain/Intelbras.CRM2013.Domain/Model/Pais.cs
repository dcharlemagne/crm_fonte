using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_pais")]
    public class Pais:IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Pais() { }

        public Pais(string organization, bool isOffline)
                : base(organization, isOffline)
            {
                RepositoryService = new RepositoryService(organization, isOffline);
            }

            public Pais(string organization, bool isOffline, object provider)
                : base(organization, isOffline, provider)
            {
                RepositoryService = new RepositoryService(organization, isOffline, provider);
            }

        #endregion

        #region Atributos
            [LogicalAttribute("itbc_paisid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("itbc_chave_integracao_pais")]
            public String ChaveIntegracao { get; set; }

            [LogicalAttribute("itbc_name")]
            public String Nome { get; set; }

            [LogicalAttribute("statecode")]
            public int? State { get; set; }
        #endregion
    }
}
