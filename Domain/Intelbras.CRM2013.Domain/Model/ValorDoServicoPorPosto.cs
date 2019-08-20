using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_valor_servico_posto")]
    public class ValorDoServicoPorPosto : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ValorDoServicoPorPosto() { }

        public ValorDoServicoPorPosto(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ValorDoServicoPorPosto(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("new_valor")]
        public decimal Valor { get; set; }
        [LogicalAttribute("new_linha_unidade_negocioid")]
        public Lookup SegmentoId { get; set; }
        [LogicalAttribute("new_clienteid")]
        public Lookup ClienteId { get; set; }
        [LogicalAttribute("new_produtoid")]
        public Lookup ProdutoId { get; set; }
        [LogicalAttribute("new_servicoid")]
        public Lookup ServicoId { get; set; }
        [LogicalAttribute("new_data_inicio")]
        public DateTime? DataInicio { get; set; }
        [LogicalAttribute("new_data_final")]
        public DateTime? DataFinal { get; set; }

        #endregion

        #region Métodos



        #endregion
    }
}