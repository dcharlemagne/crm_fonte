using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_pausa_de_tarefa")]
    public class PausaTarefa : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public PausaTarefa() { }

        public PausaTarefa(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public PausaTarefa(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public PausaTarefa(RepositoryService repositoryService) : base(repositoryService.NomeDaOrganizacao, repositoryService.IsOffline)
        {
            this.RepositoryService = repositoryService;
        }

        #endregion

        #region Atributos
        [LogicalAttribute("itbc_pausa_de_tarefaid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_data_de_inicio")]
        public DateTime DataInicio { get; set; }

        [LogicalAttribute("itbc_data_de_termino")]
        public DateTime DataTermino { get; set; }

        [LogicalAttribute("itbc_motivo")]
        public Lookup Motivo { get; set; }

        [LogicalAttribute("itbc_observacao")]
        public string Observacao { get; set; }

        [LogicalAttribute("itbc_tarefa")]
        public Lookup Tarefa { get; set; }

        #endregion
    }
}