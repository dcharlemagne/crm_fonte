using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_compromissos")]
    public class CompromissosDoPrograma : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public CompromissosDoPrograma() { }

        public CompromissosDoPrograma(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }


        public CompromissosDoPrograma(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

            [LogicalAttribute("itbc_compromissosid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("itbc_name")]
            public String Nome { get; set; }

            [LogicalAttribute("itbc_codigo")]
            public int? Codigo { get; set; }

            [LogicalAttribute("itbc_tipodemonitoramento")]
            public int? TipoMonitoramento { get; set; }

        #endregion
    }
}
