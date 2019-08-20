using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_metaporsegmento")]
    public class MetadaUnidadeporSegmento : DomainBase
    {
        #region Construtores
        public MetadaUnidadeporSegmento(string organization, bool isOffline)
            : base(organization, isOffline)
        {
        }
        public MetadaUnidadeporSegmento(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_metaporsegmentoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_Ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("itbc_MetaPlanejada")]
        public decimal? MetaPlanejada { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_metaportrimestre")]
        public Lookup MetaporTrimestre { get; set; }

        [LogicalAttribute("itbc_UltAtualizacaoIntegracao")]
        public DateTime? UltAtualizacaoIntegracao { get; set; }

        [LogicalAttribute("itbc_trimestre")]
        public int? Trimestre { get; set; }

        [LogicalAttribute("itbc_UnidadedeNegocioId")]
        public Lookup UnidadedeNegocios { get; set; }

        [LogicalAttribute("itbc_SegmentoId")]
        public Lookup Segmento { get; set; }

        [LogicalAttribute("itbc_MetaRealizada")]
        public decimal? MetaRealizada { get; set; }

        [LogicalAttribute("itbc_IntegradoPor")]
        public String IntegradoPor { get; set; }

        [LogicalAttribute("itbc_UsuarioIntegracao")]
        public String UsuarioIntegracao { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }

        #endregion
    }
}

