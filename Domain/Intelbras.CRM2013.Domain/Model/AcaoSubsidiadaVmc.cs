using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_acaosubsidiadavmc")]
    public class AcaoSubsidiadaVmc : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public AcaoSubsidiadaVmc() { }

        public AcaoSubsidiadaVmc(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }


        public AcaoSubsidiadaVmc(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
            [LogicalAttribute("itbc_acaosubsidiadavmcid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("itbc_name")]
            public String Nome { get; set; }

            [LogicalAttribute("itbc_codigo")]
            public int? Codigo { get; set; }

           

        #endregion
    }
}
