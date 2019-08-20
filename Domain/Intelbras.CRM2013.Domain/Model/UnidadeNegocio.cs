using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("businessunit")]
    public class UnidadeNegocio : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public UnidadeNegocio() { }

        public UnidadeNegocio(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public UnidadeNegocio(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public UnidadeNegocio(RepositoryService repositoryService) : base(repositoryService.NomeDaOrganizacao, repositoryService.IsOffline, repositoryService.Provider)
        {
            RepositoryService = repositoryService;
        }
        #endregion

        #region Atributos
        [LogicalAttribute("businessunitid")]
        public Guid? ID { get; set; }


        // Booleand
        [LogicalAttribute("itbc_participadopci")]
        public bool? ParticipaProgramaPci { get; set; }
        


        [LogicalAttribute("address1_city")]
        public String CidadeCobranca { get; set; }

        [LogicalAttribute("itbc_chave_integracao")]
        public String ChaveIntegracao { get; set; }

        [LogicalAttribute("address1_country")]
        public String PaisCobranca { get; set; }

        [LogicalAttribute("address1_line1")]
        public String Rua1Cobranca { get; set; }

        [LogicalAttribute("address1_line2")]
        public String Rua2Cobranca { get; set; }

        [LogicalAttribute("address1_line3")]
        public String Rua3Cobranca { get; set; }

        [LogicalAttribute("address1_postalcode")]
        public String CEPCobranca { get; set; }

        [LogicalAttribute("address1_stateorprovince")]
        public String EstadoCobranca { get; set; }

        [LogicalAttribute("address1_telephone1")]
        public String TelefonePrincipal { get; set; }

        [LogicalAttribute("address1_telephone2")]
        public String Telefone2 { get; set; }

        [LogicalAttribute("address1_telephone3")]
        public String Telefone3 { get; set; }

        [LogicalAttribute("address2_city")]
        public String CidadeEntrega { get; set; }

        [LogicalAttribute("address2_country")]
        public String PaisEntrega { get; set; }

        [LogicalAttribute("address2_line1")]
        public String Rua1Entrega { get; set; }

        [LogicalAttribute("address2_line2")]
        public String Rua2Entrega { get; set; }

        [LogicalAttribute("address2_line3")]
        public String Rua3Entrega { get; set; }

        [LogicalAttribute("address2_postalcode")]
        public String CEPEndereco2 { get; set; }

        [LogicalAttribute("address2_stateorprovince")]
        public String EstadoEndereco2 { get; set; }

        [LogicalAttribute("divisionname")]
        public String Divisao { get; set; }

        [LogicalAttribute("emailaddress")]
        public String Email { get; set; }

        [LogicalAttribute("name")]
        public String Nome { get; set; }

        [LogicalAttribute("parentbusinessunitid")]
        public Lookup NegocioPrimario { get; set; }

        [LogicalAttribute("websiteurl")]
        public String Site { get; set; }

        // [LogicalAttribute("businessunitid")]
        // public String CodigoUnidadeNegocio { get; set; }

        #endregion
    }
}
