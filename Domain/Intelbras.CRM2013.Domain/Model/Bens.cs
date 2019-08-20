using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_imoveisdaempresa")]
    public class Bens : DomainBase
    {
        #region Construtores
        
            private RepositoryService RepositoryService { get; set; }

        public Bens() { }

        public Bens(string organization, bool isOffline)
                : base(organization, isOffline)
            {
                RepositoryService = new RepositoryService(organization, isOffline);
            }

            public Bens(string organization, bool isOffline, object provider)
                : base(organization, isOffline, provider)
            {
                RepositoryService = new RepositoryService(organization, isOffline, provider);
            }

        #endregion

        #region Atributos
            [LogicalAttribute("itbc_imoveisdaempresaid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("itbc_accountid")]
            public Lookup Conta { get; set; }

            [LogicalAttribute("itbc_contactid")]
            public Lookup Contato { get; set; }

            [LogicalAttribute("itbc_address1_number")]
            public String Numero { get; set; }

            [LogicalAttribute("itbc_address1_street")]
            public String Endereco {get; set;}

            [LogicalAttribute("itbc_bairro")]
            public String Bairro {get; set;}
        
            [LogicalAttribute("itbc_cep")]
            public String CEP {get; set;}
        
            [LogicalAttribute("itbc_cidade")]
            public Lookup Cidade {get; set;}
        
            [LogicalAttribute("itbc_complemento")]
            public String Complemento {get; set;}
        
            [LogicalAttribute("itbc_estado")]
            public Lookup Estado {get; set;}
        
            [LogicalAttribute("itbc_matricula")]
            public String Matrícula {get; set;}
        
            [LogicalAttribute("itbc_name")]
            public String Nome {get; set;}
        
            [LogicalAttribute("itbc_onus")]
            public Decimal? Onus {get; set;}
        
            [LogicalAttribute("itbc_pais")]
            public Lookup Pais {get; set;}
        
            [LogicalAttribute("itbc_tipo_imovel")]
            public int? TipoImovel {get; set;}
        
            [LogicalAttribute("itbc_valor")]
            public Decimal? Valor {get; set;}
        
            [LogicalAttribute("transactioncurrencyid")]
            public Lookup Moeda { get; set; }

            [LogicalAttribute("itbc_naturezaproprietario")]
            public int? NaturezaProprietario { get; set; }

        #endregion
    }
}
