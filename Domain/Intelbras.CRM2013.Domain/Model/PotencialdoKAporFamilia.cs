using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_metadokaporfamilia")]
    public class PotencialdoKAporFamilia : DomainBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public PotencialdoKAporFamilia(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public PotencialdoKAporFamilia(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_metadokaporfamiliaid")]
        public Guid? ID { get; set; }
        

        // int
        [LogicalAttribute("itbc_Ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("itbc_Trimestre")]
        public int? Trimestre { get; set; }
        
        // decimal
        [LogicalAttribute("itbc_MetaPlanejada")]
        public decimal? PotencialPlanejado { get; set; }

        [LogicalAttribute("itbc_MetaRealizada")]
        public decimal? PotencialRealizado { get; set; }


        // Lookup
        [LogicalAttribute("itbc_MetadoKAporSegmentoId")]
        public Lookup PotencialdoKAporSegmento { get; set; }
        
        [LogicalAttribute("itbc_familiadeprodutoid")]
        public Lookup FamiliadeProduto { get; set; }
        
        [LogicalAttribute("itbc_KAouRepresentanteId")]
        public Lookup KAouRepresentante { get; set; }
        
        [LogicalAttribute("itbc_UnidadedeNegocioId")]
        public Lookup UnidadedeNegocio { get; set; }
        
        [LogicalAttribute("itbc_SegmentoId")]
        public Lookup Segmento { get; set; }


        // String
        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }
        
        [LogicalAttribute("itbc_IntegradoPor")]
        public string IntegradoPor { get; set; }
        
        [LogicalAttribute("itbc_UsuarioIntegracao")]
        public string UsuarioIntegracao { get; set; }


        // DateTime
        [LogicalAttribute("itbc_UltAtualizacaoIntegracao")]
        public DateTime? UltAtualizacaoIntegracao { get; set; }

        #endregion
    }
}