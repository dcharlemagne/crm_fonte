using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_permissao_usuario_servico")]
    public class ContratosAssociados : DomainBase
    {
        #region Atributes
        [LogicalAttribute("new_contratoid")]
        public Guid? ContratoId { get; set; }
        [LogicalAttribute("new_clienteid")]
        public Guid? ClienteId { get; set; }
        [LogicalAttribute("new_contatoid")]
        public Guid? ContatoId { get; set; }
        #endregion

        private RepositoryService RepositoryService { get; set; }

        public ContratosAssociados() { }

        public ContratosAssociados(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ContratosAssociados(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
    }
}
