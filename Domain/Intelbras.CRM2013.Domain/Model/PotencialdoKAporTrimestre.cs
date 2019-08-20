using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_metatrimestreka")]
    public class PotencialdoKAporTrimestre : DomainBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public PotencialdoKAporTrimestre(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public PotencialdoKAporTrimestre(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_metatrimestrekaid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }


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


        // lookup
        [LogicalAttribute("itbc_keyaccountreprid")]
        public Lookup KeyAccountRepresentante { get; set; }

        [LogicalAttribute("itbc_unidadenegocioid")]
        public Lookup UnidadedeNegocio { get; set; }

        [LogicalAttribute("itbc_MetadoKARepresentanteId")]
        public Lookup PotencialdoKARepresentante { get; set; }

        #endregion
    }
}