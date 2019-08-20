using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_metadocanal")]
    public class MetadoCanal : DomainBase
    {
        #region Construtores

        public MetadoCanal(string organization, bool isOffline)
            : base(organization, isOffline)
        {
        }

        public MetadoCanal(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
        }

        #endregion

        #region Atributos

        [LogicalAttribute("itbc_metadocanalid")]
        public Guid? ID { get; set; }



        // Decimal
        [LogicalAttribute("itbc_MetaPlanejada")]
        public decimal? MetaPlanejada { get; set; }

        [LogicalAttribute("itbc_MetaRealizada")]
        public decimal? MetaRealizada { get; set; }



        // String
        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }
        


        // Lookup
        [LogicalAttribute("itbc_unidadedenegocioid")]
        public Lookup UnidadedeNegocio { get; set; }
        
        [LogicalAttribute("itbc_CanalId")]
        public Lookup Canal { get; set; }



        // Int        
        [LogicalAttribute("itbc_ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }

        #endregion
    }
}