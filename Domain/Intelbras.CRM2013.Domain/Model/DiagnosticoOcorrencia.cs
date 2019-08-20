using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_diagnostico_ocorrencia")]
    public class DiagnosticoOcorrencia: DomainBase
    {

        #region Atributos

        [LogicalAttribute("new_extrato_logistica_reversaid")]
        public Lookup ExtratoLogisticaReversaid { get; set; }

        private Model.Conta cliente;
        public Model.Conta Cliente
        {
            get { return cliente; }
            set { cliente = value; }
        }
        [LogicalAttribute("new_servicoid")]
        public Lookup ServicoId { get; set; }
        [LogicalAttribute("new_produtoid")]
        public Lookup ProdutoId { get; set; }
        [LogicalAttribute("new_valor_mao_obra")]
        public Decimal? MaoDeObra { get; set; }

        #endregion

        #region Construtor

        private RepositoryService RepositoryService { get; set; }

        public DiagnosticoOcorrencia() { }

        public DiagnosticoOcorrencia(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public DiagnosticoOcorrencia(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Metodos

        public List<Diagnostico> ListarPagamentoPorAstec(int dia)
        {
            return RepositoryService.Diagnostico.ListarPagamentoPorAstec(dia);
        }

        #endregion
    }
}
