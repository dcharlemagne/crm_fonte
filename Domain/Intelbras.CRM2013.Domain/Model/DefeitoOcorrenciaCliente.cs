using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    [LogicalEntity("new_defeito_ocorrencia_cliente")]
    public class DefeitoOcorrenciaCliente : DomainBase
    {
        private RepositoryService RepositoryService { get; set; }

        public DefeitoOcorrenciaCliente() { }

        public DefeitoOcorrenciaCliente(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public DefeitoOcorrenciaCliente(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        [LogicalAttribute("new_ocorrenciaid")]
        public Lookup OcorrenciaId { get; set; }
        [LogicalAttribute("new_defeitoid")]
        public Lookup DefeitoId { get; set; }
        [LogicalAttribute("new_name")]
        public String Nome { get; set; }
    }
}
