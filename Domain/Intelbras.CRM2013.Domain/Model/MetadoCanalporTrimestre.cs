using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_metatrimestrecanal")]
    public class MetadoCanalporTrimestre : DomainBase
    {
        public MetadoCanalporTrimestre(string organization, bool isOffline) : base(organization, isOffline) { }

        public MetadoCanalporTrimestre(string organization, bool isOffline, object provider) : base(organization, isOffline, provider) { }

        #region Atributos

        [LogicalAttribute("itbc_metatrimestrecanalid")]
        public Guid? ID { get; set; }



        // String 
        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }



        // Decimal
        [LogicalAttribute("itbc_metaplanejada")]
        public decimal? MetaPlanejada { get; set; }

        [LogicalAttribute("itbc_metarealizada")]
        public decimal? MetaRealizada { get; set; }



        // Lookup
        [LogicalAttribute("itbc_unidadenegocioid")]
        public Lookup UnidadedeNegocio { get; set; }

        [LogicalAttribute("itbc_metadocanalid")]
        public Lookup MetaCanal { get; set; }

        [LogicalAttribute("itbc_canal")]
        public Lookup Canal { get; set; }



        // Int
        [LogicalAttribute("itbc_ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("itbc_trimestre")]
        public int? Trimestre { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }

        #endregion
    }
}
