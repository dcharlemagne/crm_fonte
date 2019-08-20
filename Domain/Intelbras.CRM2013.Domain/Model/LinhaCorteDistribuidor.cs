using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_linhadecorte")]
    public class LinhaCorteDistribuidor:DomainBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public LinhaCorteDistribuidor() { }

        public LinhaCorteDistribuidor(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public LinhaCorteDistribuidor(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }        

        #endregion

        #region Atributos
        [LogicalAttribute("itbc_linhadecorteid")]
        public Guid? ID { get; set; }  
     
        [LogicalAttribute("itbc_businessunitid")]
        public Lookup UnidadeNegocios { get; set; }  

        [LogicalAttribute("itbc_LinhadeCorteSemestral")]
        public Decimal? LinhaCorteSemestral { get; set; }  

        [LogicalAttribute("itbc_LinhadeCorteTrimestral")]
        public Decimal? LinhaCorteTrimestral { get; set; }  

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }  
     
        [LogicalAttribute("transactioncurrencyid")]
        public Lookup Moeda { get; set; }

        [LogicalAttribute("statecode")]
        public int? Status { get; set; }

        [LogicalAttribute("itbc_capitalouinterior")]
        public int? CapitalOuInterior { get; set; }

        [LogicalAttribute("itbc_classificacaoid")]
        public Lookup Classificacao { get; set; }

        [LogicalAttribute("itbc_categoriaid")]
        public Lookup Categoria { get; set; }
        #endregion
    }
}
