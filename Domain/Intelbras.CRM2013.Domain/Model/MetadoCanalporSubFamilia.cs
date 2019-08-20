using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_metadocanalporsubfamilia")]
    public class MetadoCanalporSubFamilia : DomainBase
    {
        #region Construtores

        public MetadoCanalporSubFamilia(string organization, bool isOffline) : base(organization, isOffline) { }


        public MetadoCanalporSubFamilia(string organization, bool isOffline, object provider) : base(organization, isOffline, provider) { }
        
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_metadocanalporsubfamiliaid")]
        public Guid? ID { get; set; }



        // Decimal
        [LogicalAttribute("itbc_metaplanejada")]
        public decimal? MetaPlanejada { get; set; }

        [LogicalAttribute("itbc_metarealizada")]
        public decimal? MetaRealizada { get; set; }



        // Lookup
        [LogicalAttribute("itbc_unidadedenegociosid")]
        public Lookup UnidadedeNegocio { get; set; }

        [LogicalAttribute("itbc_FamiliaId")]
        public Lookup Familia { get; set; }

        [LogicalAttribute("itbc_MetadoCanalporFamiliaId")]
        public Lookup MetadoCanalporFamilia { get; set; }

        [LogicalAttribute("itbc_subfamiliaid")]
        public Lookup SubFamilia { get; set; }

        [LogicalAttribute("itbc_CanalId")]
        public Lookup Canal { get; set; }

        [LogicalAttribute("itbc_SegmentoId")]
        public Lookup Segmento { get; set; }



        // String
        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }



        // Int
        [LogicalAttribute("itbc_Ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("itbc_Trimestre")]
        public int? Trimestre { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }

        #endregion
    }
}