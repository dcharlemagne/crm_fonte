using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("incidentresolution")]
    public class SolucaoOcorrencia : DomainBase
    {
        private RepositoryService RepositoryService { get; set; }

        public SolucaoOcorrencia() { }

        public SolucaoOcorrencia(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public SolucaoOcorrencia(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        [LogicalAttribute("incidentid")]
        public Guid OcorrenciaId { get; set; }
        [LogicalAttribute("actualend")]
        public DateTime DataHoraConclusao { get; set; }
        [LogicalAttribute("subject")]
        public String Nome { get; set; }

    }
}
