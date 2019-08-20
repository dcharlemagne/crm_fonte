using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_metaportrimestre")]
    public class MetadaUnidadeporTrimestre : DomainBase
    {
        #region Construtores
        public MetadaUnidadeporTrimestre(string organization, bool isOffline)
            : base(organization, isOffline)
        {
        }

        public MetadaUnidadeporTrimestre(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_metaportrimestreid")]
        public Guid? ID { get; set; }


        [LogicalAttribute("itbc_Ano")]
        public int? Ano { get; set; }


        [LogicalAttribute("itbc_MetaPlanejada")]
        public decimal? MetaPlanejada { get; set; }


        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }


        [LogicalAttribute("itbc_metas")]
        public Lookup MetadaUnidade { get; set; }


        [LogicalAttribute("itbc_unidadedenegocioid")]
        public Lookup UnidadedeNegocio { get; set; }


        [LogicalAttribute("itbc_Trimestre")]
        public Int32? Trimestre { get; set; }


        [LogicalAttribute("itbc_UltAtualizacaoIntegracao")]
        public DateTime? UltAtualizacaoIntegracao { get; set; }


        [LogicalAttribute("itbc_MetaRealizada")]
        public decimal? MetaRealizada { get; set; }


        [LogicalAttribute("itbc_IntegradoPor")]
        public String IntegradoPor { get; set; }


        [LogicalAttribute("itbc_UsuarioIntegracao")]
        public String UsuarioIntegracao { get; set; }


        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }

        [LogicalAttribute("statecode")]
        public int? StateCode { get; set; }

        #endregion
    }
}