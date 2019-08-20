using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_historicocomprassegmento")]
    public class HistoricoComprasSegmento : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public HistoricoComprasSegmento() { }

        public HistoricoComprasSegmento(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public HistoricoComprasSegmento(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("itbc_historicocomprassegmentoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_unidadedenegocioid")]
        public Lookup UnidadeNegocio { get; set; }

        [LogicalAttribute("itbc_segmentoid")]
        public Lookup Segmento { get; set; }

        [LogicalAttribute("itbc_ano")]
        public Int32 Ano { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_valor")]
        public decimal? Valor { get; set; }

        [LogicalAttribute("itbc_trimestrenew")]
        public int? Trimestre { get; set; }

        [LogicalAttribute("itbc_historicotrimestreid")]
        public Lookup TrimestreRelacionamento { get; set; }

        [LogicalAttribute("itbc_quantidade")]
        public Int32 Quantidade { get;set;}

        #endregion
    }
}
