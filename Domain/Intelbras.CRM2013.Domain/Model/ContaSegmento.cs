using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    [LogicalEntity("itbc_contasegmento")]
    public class ContaSegmento : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ContaSegmento() { }

        public ContaSegmento(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ContaSegmento(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public ContaSegmento(RepositoryService repositoryService) : base(repositoryService.NomeDaOrganizacao, repositoryService.IsOffline)
        {
            this.RepositoryService = repositoryService;
        }

        #endregion

        #region Atributos
        [LogicalAttribute("itbc_contasegmentoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_contaid")]
        public Lookup Conta { get; set; }

        [LogicalAttribute("itbc_segmentoid")]
        public Lookup Segmento { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }

        [LogicalAttribute("itbc_mais_verde")]
        public bool MaisVerde { get; set; }

        #endregion

    }
}
