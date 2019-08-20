using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_historicocomprastrimestre")]
    public class HistoricoComprasTrimestre : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public HistoricoComprasTrimestre() { }

        public HistoricoComprasTrimestre(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public HistoricoComprasTrimestre(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("itbc_historicocomprastrimestreid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_unidadedenegocio")]
        public Lookup UnidadeNegocio { get; set; }

        [LogicalAttribute("itbc_historicounidadeid")]
        public Lookup HistoricoCompraUnidade { get; set; }

        [LogicalAttribute("itbc_ano")]
        public Int32? Ano { get; set; }

        [LogicalAttribute("itbc_trimestre")]
        public int? Trimestre { get; set; }        

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_valor")]
        public decimal? Valor { get; set; }

        #endregion
    }
}
