using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;



namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_portfoliokeyaccountrepresentantes")]
    public class PortfoliodoKeyAccountRepresentantes : DomainBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public PortfoliodoKeyAccountRepresentantes(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public PortfoliodoKeyAccountRepresentantes(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_portfoliokeyaccountrepresentantesid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_contatoid")]
        public Lookup KeyAccountRepresentante { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_UnidadedeNegocioId")]
        public Lookup UnidadedeNegocio { get; set; }

        [LogicalAttribute("itbc_ult_atualizacao_Integracao")]
        public DateTime? UltAtualizacaoIntegracao { get; set; }

        [LogicalAttribute("itbc_usuario_integracao")]
        public String UsuarioIntegracao { get; set; }

        [LogicalAttribute("itbc_AssistentedeAdministracaodeVendas")]
        public Lookup AssistentedeAdministracaodeVendas { get; set; }

        [LogicalAttribute("itbc_SegmentoId")]
        public Lookup Segmento { get; set; }

        [LogicalAttribute("itbc_SupervisordeVendas")]
        public Lookup SupervisordeVendas { get; set; }

        [LogicalAttribute("itbc_integradopor")]
        public String IntegradoPor { get; set; }

        [LogicalAttribute("statuscode")]
        public int? Status { get; set; }

        [LogicalAttribute("statecode")]
        public int? State { get; set; }
        #endregion
    }
}

