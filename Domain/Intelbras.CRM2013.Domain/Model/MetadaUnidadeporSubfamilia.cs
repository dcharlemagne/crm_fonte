using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_metasubfamiliasegmentotrimestre")]
    public class MetadaUnidadeporSubfamilia : DomainBase
    {
        #region Construtores
        public MetadaUnidadeporSubfamilia(string organization, bool isOffline)
        : base(organization, isOffline)
        {
        }
        public MetadaUnidadeporSubfamilia(string organization, bool isOffline, object provider)
        : base(organization, isOffline, provider)
        {
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_metasubfamiliasegmentotrimestreid")]
        public Guid? ID { get; set; }


        // String 
        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }



        // Lookup
        [LogicalAttribute("itbc_FamiliaId")]
        public Lookup Familia { get; set; }

        [LogicalAttribute("itbc_Subfamilia")]
        public Lookup Subfamilia { get; set; }

        [LogicalAttribute("itbc_UnidadedeNegocioId")]
        public Lookup UnidadedeNegocio { get; set; }

        [LogicalAttribute("itbc_metafamiliasegmentotrimestre")]
        public Lookup MetadaFamilia { get; set; }

        [LogicalAttribute("itbc_SegmentoId")]
        public Lookup Segmento { get; set; }
        


        // Decimal
        [LogicalAttribute("itbc_MetaRealizada")]
        public decimal? MetaRealizada { get; set; }

        [LogicalAttribute("itbc_MetaPlanejada")]
        public decimal? MetaPlanejada { get; set; }


        // Int
        [LogicalAttribute("itbc_Ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }

        [LogicalAttribute("itbc_trimestre")]
        public int? Trimestre { get; set; }


        #endregion
    }
}