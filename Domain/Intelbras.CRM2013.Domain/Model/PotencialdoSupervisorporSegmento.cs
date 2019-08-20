using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_potencial_supervisorporsegmento")]
    public class PotencialdoSupervisorporSegmento : DomainBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public PotencialdoSupervisorporSegmento(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public PotencialdoSupervisorporSegmento(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_potencial_supervisorporsegmentoid")]
        public Guid? ID { get; set; }



        // int
        [LogicalAttribute("itbc_ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("itbc_trimestre")]
        public int? Trimestre { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }



        // Lookup
        [LogicalAttribute("itbc_unidadedenegociosid")]
        public Lookup UnidadedeNegocios { get; set; }

        [LogicalAttribute("itbc_metatrimestresupervisorid")]
        public Lookup PotencialdoTrimestreSupervisor { get; set; }
        
        [LogicalAttribute("itbc_SegmentoId")]
        public Lookup Segmento { get; set; }
        
        [LogicalAttribute("itbc_supervisor")]
        public Lookup Supervisor { get; set; }



        // string
        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_integradopor")]
        public String IntegradoPor { get; set; }
        

        [LogicalAttribute("itbc_usuariointegracao")]
        public String UsuarioIntegracao { get; set; }



        // decimal
        [LogicalAttribute("itbc_potencialplanejado")]
        public decimal? PotencialPlanejado { get; set; }

        [LogicalAttribute("itbc_potencialrealizado")]
        public decimal? PotencialRealizado { get; set; }



        // Datetime
        [LogicalAttribute("itbc_ultatualizacaointegracao")]
        public DateTime? UltAtualizacaoIntegracao { get; set; }

        #endregion
    }
}