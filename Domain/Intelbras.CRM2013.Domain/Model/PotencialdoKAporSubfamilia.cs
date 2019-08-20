using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_metadokaporsubfamilia")]
    public class PotencialdoKAporSubfamilia : DomainBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public PotencialdoKAporSubfamilia(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public PotencialdoKAporSubfamilia(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_metadokaporsubfamiliaid")]
        public Guid? ID { get; set; }


        // Lookup
        [LogicalAttribute("itbc_MetadoKAporFamiliaId")]
        public Lookup PotencialdoKAporFamilia { get; set; }

        [LogicalAttribute("itbc_FamiliadeProdutoId")]
        public Lookup FamiliadeProduto { get; set; }

        [LogicalAttribute("itbc_KAouRepresentanteId")]
        public Lookup KAouRepresentante { get; set; }

        [LogicalAttribute("itbc_UnidadedeNegocioId")]
        public Lookup UnidadedeNegocio { get; set; }

        [LogicalAttribute("itbc_SubfamiliadeProdutoId")]
        public Lookup SubfamiliadeProduto { get; set; }

        [LogicalAttribute("itbc_SegmentoId")]
        public Lookup Segmento { get; set; }


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


        // string
        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }

        [LogicalAttribute("itbc_IntegradoPor")]
        public String IntegradoPor { get; set; }

        [LogicalAttribute("itbc_UsuarioIntegracao")]
        public String UsuarioIntegracao { get; set; }


        // Datetime
        [LogicalAttribute("itbc_UltAtualizacaoIntegracao")]
        public DateTime? UltAtualizacaoIntegracao { get; set; }

        #endregion
    }
}