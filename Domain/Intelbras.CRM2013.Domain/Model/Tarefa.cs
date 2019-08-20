using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("task")]
    public class Tarefa : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Tarefa() { }

        public Tarefa(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Tarefa(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public Tarefa(RepositoryService repositoryService) : base(repositoryService.NomeDaOrganizacao, repositoryService.IsOffline)
        {
            this.RepositoryService = repositoryService;
        }

        #endregion


        #region Atributos
        [LogicalAttribute("activityid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("subject")]
        public String Assunto { get; set; }

        [LogicalAttribute("scheduledend")]
        public DateTime? Conclusao { get; set; }

        [LogicalAttribute("scheduledstart")]
        public DateTime? DataInicial { get; set; }

        [LogicalAttribute("itbc_tipoatividadeid")]
        public Lookup TipoDeAtividade { get; set; }

        [LogicalAttribute("itbc_resultado")]
        public int? Resultado { get; set; }

        [LogicalAttribute("actualdurationminutes")]
        public int? Duracao { get; set; }

        [LogicalAttribute("itbc_ordem")]
        public int? Ordem { get; set; }

        [LogicalAttribute("itbc_descricaosolicitacao")]
        public String DescricaoSolicitacao { get; set; }

        [LogicalAttribute("description")]
        public String Descricao { get; set; }

        [LogicalAttribute("itbc_pareceresanteriores")]
        public String PareceresAnteriores { get; set; }

        [LogicalAttribute("regardingobjectid")]
        public Lookup ReferenteA { get; set; }

        [LogicalAttribute("statuscode")]
        public int? Status { get; set; }

        [LogicalAttribute("statecode")]
        public int? State { get; set; }

        [LogicalAttribute("createdon")]
        public DateTime? CriadoEm { get; set; }

        [LogicalAttribute("modifiedon")]
        public DateTime? ModificadoEm { get; set; }

        [LogicalAttribute("prioritycode")]
        public int? Prioridade { get; set; }

        [LogicalAttribute("subcategory")]
        public String Subcategoria { get; set; }

        [LogicalAttribute("category")]
        public String Categoria { get; set; }

        [LogicalAttribute("actualend")]
        public DateTime? TerminoReal { get; set; }

        [LogicalAttribute("itbc_tempo_resposta")]
        public Decimal? TempoResposta { get; set; }

        [LogicalAttribute("itbc_tempo_atuacao")]
        public decimal? TempoAtuacao { get; set; }

        [LogicalAttribute("itbc_motivo_pausa")]
        public string MotivoPausa { get; set; }


        #endregion
    }
}
