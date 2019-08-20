using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_metadokaporsegmento")]
    public class PotencialdoKAporSegmento : DomainBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public PotencialdoKAporSegmento(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public PotencialdoKAporSegmento(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("itbc_metadokaporsegmentoid")]
        public Guid? ID { get; set; }
        

        // int
        [LogicalAttribute("itbc_Ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("itbc_Trimestre")]
        public int? Trimestre { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }


        // decimal
        [LogicalAttribute("itbc_MetaPlanejada")]
        public decimal? PotencialPlanejado { get; set; }

        [LogicalAttribute("itbc_MetaRealizada")]
        public decimal? PotencialRealizado { get; set; }


        // Lookup
        [LogicalAttribute("itbc_KAouRepresentanteId")]
        public Lookup KAouRepresentante { get; set; }

        [LogicalAttribute("itbc_SegmentoId")]
        public Lookup Segmento { get; set; }

        [LogicalAttribute("itbc_metatrimestrekaid")]
        public Lookup PotencialKaTrimestre { get; set; }

        [LogicalAttribute("itbc_UnidadedeNegocioId")]
        public Lookup UnidadedeNegocio { get; set; }

        [LogicalAttribute("itbc_supervisorid")]
        public Lookup Supervisor { get; set; }


        // String
        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_IntegradoPor")]
        public String IntegradoPor { get; set; }

        [LogicalAttribute("itbc_UsuarioIntegracao")]
        public String UsuarioIntegracao { get; set; }


        // DateTime
        [LogicalAttribute("itbc_UltAtualizacaoIntegracao")]
        public DateTime? UltAtualizacaoIntegracao { get; set; }

        #endregion
    }
}