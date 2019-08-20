using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_prioridade_ligacao_callcenter")]
    public class PrioridadeLigacaoCallCenter : DomainBase
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public PrioridadeLigacaoCallCenter() { }

        public PrioridadeLigacaoCallCenter(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public PrioridadeLigacaoCallCenter(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("new_prioridade_ligacao_callcenterid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("new_cpfcnpj")]
        public string CpfCnpj { get; set; }

        [LogicalAttribute("new_nome_fila")]
        public string NomeFila { get; set; }

        [LogicalAttribute("new_prioridade")]
        public int? Prioridade { get; set; }

        [LogicalAttribute("statecode")]
        public int StateCode { get; set; }

        [LogicalAttribute("statuscode")]
        public int? StatusCode { get; set; }

        [LogicalAttribute("new_name")]
        public string Nome { get; set; }

        #endregion

    }
}
