using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_produtopoliticacomercial")]
    public class ProdutoPoliticaComercial : DomainBase
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ProdutoPoliticaComercial() { }

        public ProdutoPoliticaComercial(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public ProdutoPoliticaComercial(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_produtopoliticacomercialid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome {get; set;}

        [LogicalAttribute("itbc_politicacomercialid")]
        public Lookup PoliticaComercial { get; set; }

        [LogicalAttribute("itbc_familprodid")]
        public Lookup FamiliaProduto { get; set; }

        [LogicalAttribute("itbc_productid")]
        public Lookup Produto { get; set; }

        [LogicalAttribute("itbc_quantidade_inicial")]
        public int? QtdInicial { get; set; }

        [LogicalAttribute("itbc_quantidade_final")]
        public int? QtdFinal { get; set; }

        [LogicalAttribute("itbc_fator")]
        public double? Fator { get; set; }

        [LogicalAttribute("statuscode")]
        public int? Status { get; set; }

        [LogicalAttribute("itbc_data_inicio_vigencia")]
        public DateTime? DataInicioVigencia { get; set; }

        [LogicalAttribute("itbc_data_fim_vigencia")]
        public DateTime? DataFimVigencia { get; set; }

        
        
        

        #endregion
    }
}
