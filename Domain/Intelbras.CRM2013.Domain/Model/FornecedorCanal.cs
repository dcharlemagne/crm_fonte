using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_fornecedordocanal")]
    public class FornecedorCanal:IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public FornecedorCanal() { }

        public FornecedorCanal(string organization, bool isOffline)
                : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline); 
        }


        public FornecedorCanal(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }        
               
        #endregion

        #region Atributos
            [LogicalAttribute("itbc_fornecedordocanalid")]
            public Guid? ID {get; set;}
        
            [LogicalAttribute("itbc_accountid")]
            public Lookup Canal{get; set;}

            [LogicalAttribute("itbc_contato")]
            public String Contato{get; set;}

            [LogicalAttribute("itbc_name")]
            public String Nome{get; set;}

            [LogicalAttribute("itbc_telefone")]
            public String Telefone{get; set;}
        
            [LogicalAttribute("statecode")]
            public int? State { get; set; }
        #endregion
    }
}
