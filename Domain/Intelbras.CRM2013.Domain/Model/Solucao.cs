using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_servico_assistencia_tecnica")]
    public class Solucao : DomainBase
    {
        [LogicalAttribute("new_gera_troca_peca")]
        public bool GeraTroca { get; set; }

        private RepositoryService RepositoryService { get; set; }

        public Solucao() { }

        public Solucao(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Solucao(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        [LogicalAttribute("new_name")]
        public String Nome { get; set; }
    }
}
