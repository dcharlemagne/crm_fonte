using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_metatrimsupervisor")]
    public class PotencialdoSupervisorporTrimestre : IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public PotencialdoSupervisorporTrimestre(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public PotencialdoSupervisorporTrimestre(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("itbc_metatrimsupervisorid")]
        public Guid? ID { get; set; }



        // Lookup
        [LogicalAttribute("itbc_unidadedenegocioid")]
        public Lookup UnidadedeNegocio { get; set; }
        
        [LogicalAttribute("itbc_potencialdosupervisorid")]
        public Lookup PotencialSupervisor { get; set; }

        [LogicalAttribute("itbc_supervisorid")]
        public Lookup Supervisor { get; set; }



        // string 
        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }


        // Int
        [LogicalAttribute("itbc_ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("itbc_trimestre")]
        public int? Trimestre { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }



        // Decimal
        [LogicalAttribute("itbc_metaplanejada")]
        public decimal? PotencialPlanejado { get; set; }

        [LogicalAttribute("itbc_metarealizada")]
        public decimal? PotencialRealizado { get; set; }

        #endregion
    }
}