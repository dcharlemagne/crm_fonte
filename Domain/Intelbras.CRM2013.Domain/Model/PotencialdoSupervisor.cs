using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_potencialdosupervisor")]
    public class PotencialdoSupervisor : DomainBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public PotencialdoSupervisor(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public PotencialdoSupervisor(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_potencialdosupervisorid")]
        public Guid? ID { get; set; }


        // int
        [LogicalAttribute("itbc_ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }


        // string
        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }



        // Lookup
        [LogicalAttribute("itbc_PotencialGeradaPorId")]
        public Lookup PotencialGeradaPor { get; set; }

        //[LogicalAttribute("itbc_PotencialPorTrimestredaUnidadeId")]
        //public Lookup PotencialporTrimestredaUnidade { get; set; }

        [LogicalAttribute("itbc_unidadedenegocioId")]
        public Lookup UnidadedeNegocio { get; set; }

        [LogicalAttribute("itbc_supervisor")]
        public Lookup Supervisor { get; set; }



        // decimal
        [LogicalAttribute("itbc_PotencialPlanejado")]
        public decimal? PotencialPlanejado { get; set; }

        [LogicalAttribute("itbc_PotencialRealizado")]
        public decimal? PotencialRealizado { get; set; }



        //Datetime
        [LogicalAttribute("itbc_dataultimageracao")]
        public DateTime? DataUltimaGeracao { get; set; }

        #endregion
    }
}