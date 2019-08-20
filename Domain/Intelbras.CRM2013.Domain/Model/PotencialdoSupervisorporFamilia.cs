using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_potencialdosupervisorporfamilia")]
    public class PotencialdoSupervisorporFamilia : DomainBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public PotencialdoSupervisorporFamilia(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public PotencialdoSupervisorporFamilia(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_potencialdosupervisorporfamiliaid")]
        public Guid? ID { get; set; }


        // Int
        [LogicalAttribute("itbc_ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("itbc_trimestre")]
        public int? Trimestre { get; set; }

        // Lookup
        [LogicalAttribute("itbc_familiadoprodutoId")]
        public Lookup FamiliadoProduto { get; set; }

        [LogicalAttribute("itbc_segmentoId")]
        public Lookup Segmento { get; set; }

        [LogicalAttribute("itbc_unidadedenegocioId")]
        public Lookup UnidadedeNegocio { get; set; }

        [LogicalAttribute("itbc_potencialsupervisorporfamiliaId")]
        public Lookup PotencialdoSupervisorporSegmento { get; set; }

        [LogicalAttribute("itbc_supervisor")]
        public Lookup Supervisor { get; set; }



        // string
        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }



        // decimal
        [LogicalAttribute("itbc_potencialplanejado")]
        public decimal? PotencialPlanejado { get; set; }

        [LogicalAttribute("itbc_potencialrealizado")]
        public decimal? PotencialRealizado { get; set; }

        #endregion
    }
}