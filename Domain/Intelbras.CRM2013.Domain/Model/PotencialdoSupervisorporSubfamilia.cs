using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_potencial_supervisor_subfamilia")]
    public class PotencialdoSupervisorporSubfamilia : DomainBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public PotencialdoSupervisorporSubfamilia(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public PotencialdoSupervisorporSubfamilia(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_potencial_supervisor_subfamiliaid")]
        public Guid? ID { get; set; }



        // int
        [LogicalAttribute("itbc_ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("itbc_trimestre")]
        public int? Trimestre { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }



        // Lookup
        [LogicalAttribute("itbc_potencialsupervisorsubfamiliaId")]
        public Lookup PotencialdoSupervisorporSubfamiliaID { get; set; }

        [LogicalAttribute("itbc_unidadedenegocioId")]
        public Lookup UnidadedeNegocio { get; set; }

        [LogicalAttribute("itbc_supervisor")]
        public Lookup Supervisor { get; set; }

        [LogicalAttribute("itbc_subfamliadeprodutoId")]
        public Lookup SubfamiliadeProduto { get; set; }

        [LogicalAttribute("itbc_familiadeprodutoId")]
        public Lookup FamiliadoProduto { get; set; }

        [LogicalAttribute("itbc_segmentoId")]
        public Lookup Segmento { get; set; }



        // string 
        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }



        // decimal
        [LogicalAttribute("itbc_potencialplanejado")]
        public decimal? PotencialPlanejado { get; set; }
        
        [LogicalAttribute("itbc_PotencialRealizado")]
        public decimal? PotencialRealizado { get; set; }

        #endregion
    }
}