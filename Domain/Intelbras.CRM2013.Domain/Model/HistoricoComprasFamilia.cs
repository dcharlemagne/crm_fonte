using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_historicodecomprasporfamilia")]
    public class HistoricoComprasFamilia : DomainBase
    {
         #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public HistoricoComprasFamilia() { }

        public HistoricoComprasFamilia(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public HistoricoComprasFamilia(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("itbc_historicodecomprasporfamiliaid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_valor")]
        public decimal? Valor { get; set; }

        [LogicalAttribute("itbc_ano")]
        public Int32? Ano { get; set; }

        [LogicalAttribute("itbc_trimestre")]
        public Int32? Trimestre { get; set; }

        [LogicalAttribute("itbc_segmentoid")]
        public Lookup Segmento { get; set; }

        [LogicalAttribute("itbc_familiadeprodutoid")]
        public Lookup FamiliaDoProduto { get; set; }

        [LogicalAttribute("itbc_historicodecomprasporsegmentoid")]
        public Lookup SegmentoRelacionamento { get; set; }

        [LogicalAttribute("itbc_unidadedenegocioid")]
        public Lookup UnidadeNegocio { get; set; }

        #endregion
    }
}
