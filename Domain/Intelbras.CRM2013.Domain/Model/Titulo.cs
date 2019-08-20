using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_titulo")]
    public class Titulo : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Titulo() { }

        public Titulo(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Titulo(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("itbc_tituloid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_accountid")]
        public Lookup Conta { get; set; }

        [LogicalAttribute("itbc_businessunit")]
        public Lookup UnidadeNegocio { get; set; }

        [LogicalAttribute("itbc_carteira")]
        public String Carteira { get; set; }

        [LogicalAttribute("itbc_chaveintegracao_titulo")]
        public String ChaveIntegracaoTitulo { get; set; }

        [LogicalAttribute("itbc_contact")]
        public Lookup Representante { get; set; }

        [LogicalAttribute("itbc_contato")]
        public Lookup Contato { get; set; }

        [LogicalAttribute("itbc_data_emissao")]
        public DateTime? DataEmissao { get; set; }

        [LogicalAttribute("itbc_data_indicacao_pd")]
        public DateTime? DataIndicacaoPD { get; set; }

        [LogicalAttribute("itbc_data_liquidacao")]
        public DateTime? DataLiquidacao { get; set; }

        [LogicalAttribute("itbc_data_vencimento")]
        public DateTime? DataVencimento { get; set; }

        [LogicalAttribute("itbc_data_vencimento_original")]
        public DateTime? DataVencimentoOriginal { get; set; }

        [LogicalAttribute("itbc_especie")]
        public String Especie { get; set; }

        [LogicalAttribute("itbc_estabelecimento")]
        public Lookup Estabelecimento { get; set; }

        [LogicalAttribute("itbc_estornado")]
        public Boolean Estornado { get; set; }

        [LogicalAttribute("itbc_modulo")]
        public String Modulo { get; set; }
        #endregion
    }
}
