using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_metafamiliasegmentotrimestre")]
    public class MetadaUnidadeporFamilia : DomainBase
    {
        #region Construtores
        public MetadaUnidadeporFamilia() { }
        public MetadaUnidadeporFamilia(string organization, bool isOffline)
        : base(organization, isOffline)
        {
        }
        public MetadaUnidadeporFamilia(string organization, bool isOffline, object provider)
        : base(organization, isOffline, provider)
        {
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_metafamiliasegmentotrimestreid")]
        public Guid? ID { get; set; }
        


        // Decimal
        [LogicalAttribute("itbc_MetaPlanejada")]
        public decimal? MetaPlanejada { get; set; }

        [LogicalAttribute("itbc_MetaRealizada")]
        public decimal? MetaRealizada { get; set; }



        // String
        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }



        // Lookup
        [LogicalAttribute("itbc_SegmentoId")]
        public Lookup Segmento { get; set; }

        [LogicalAttribute("itbc_MetadoSegmentoId")]
        public Lookup MetadoSegmento { get; set; }

        [LogicalAttribute("itbc_Familiadeprodutos")]
        public Lookup Familia { get; set; }

        [LogicalAttribute("itbc_UnidadedeNegocioId")]
        public Lookup UnidadedeNegocio { get; set; }




        // Int
        [LogicalAttribute("itbc_trimestre")]
        public int? Trimestre { get; set; }

        [LogicalAttribute("itbc_Ano")]
        public int? Ano { get; set; }
        
        #endregion
    }
}