using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_metadocanalporfamilia")]
    public class MetadoCanalporFamilia : DomainBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public MetadoCanalporFamilia(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public MetadoCanalporFamilia(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_metadocanalporfamiliaid")]
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
        [LogicalAttribute("itbc_FamiliaId")]
        public Lookup Familia { get; set; }



        [LogicalAttribute("itbc_MetadoCanalporSegmentoId")]
        public Lookup MetadoCanalporSegmento { get; set; }
        
        [LogicalAttribute("itbc_CanalId")]
        public Lookup Canal { get; set; }

        [LogicalAttribute("itbc_UnidadedeNegocioId")]
        public Lookup UnidadedeNegocio { get; set; }

        [LogicalAttribute("itbc_SegmentoId")]
        public Lookup Segmento { get; set; }




        // Int
        [LogicalAttribute("itbc_ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("itbc_Trimestre")]
        public int? Trimestre { get; set; }

        #endregion
    }
}