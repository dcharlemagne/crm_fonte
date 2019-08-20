using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_sinonimos_marcas")]
    public class SinonimosMarcas:IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public SinonimosMarcas() { }

        public SinonimosMarcas(string organization, bool isOffline)
                : base(organization, isOffline)
            {
                RepositoryService = new RepositoryService(organization, isOffline);
            }

            public SinonimosMarcas(string organization, bool isOffline, object provider)
                : base(organization, isOffline, provider)
            {
                RepositoryService = new RepositoryService(organization, isOffline, provider);
            }

        #endregion

        #region Atributos
            [LogicalAttribute("itbc_sinonimos_marcasid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("itbc_name")]
            public String Nome { get; set; }

            [LogicalAttribute("itbc_sinonimo")]
            public string Sinonimo { get; set; }

            [LogicalAttribute("itbc_marca_id")]
            public Lookup Marca { get; set; }

            [LogicalAttribute("statecode")]
            public int? State { get; set; }
        #endregion
    }
}
