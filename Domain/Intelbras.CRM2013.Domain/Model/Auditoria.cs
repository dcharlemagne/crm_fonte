using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_auditoria_ocorrencia")]
    public class Auditoria : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Auditoria() { }

        public Auditoria(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Auditoria(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("new_name")]
        public String Nome { get; set; }
        [LogicalAttribute("new_justificativa")]
        public string Justificativa { get; set; }
        [LogicalAttribute("new_tipo_motivo_auditoriaid")]
        public Lookup MotivoId { get; set; }
        [LogicalAttribute("new_data_posto_ajustou_audtoria")]
        public DateTime? DataFinalizacao { get; set; }
        [LogicalAttribute("new_ocorrenciaid")]
        public Lookup OcorrenciaId { get; set; }

        #endregion
    }
}