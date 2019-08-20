using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_linhadecorterevenda")]
    public class LinhaCorteRevenda:DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public LinhaCorteRevenda() { }

        public LinhaCorteRevenda(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public LinhaCorteRevenda(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }  
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_linhadecorterevendaid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_businessunitid")]
        public Lookup UnidadeNegocio { get; set; }

        [LogicalAttribute("itbc_CategoriaId")]
        public Lookup Categoria { get; set; }

        [LogicalAttribute("itbc_ClassificacaoId")]
        public Lookup Classificação { get; set; }
                
        [LogicalAttribute("itbc_LinhadeCorteSemestral")]
        public Decimal? LinhaCorteSemestral { get; set; }

        [LogicalAttribute("itbc_LinhadeCorteTrimestral")]
        public Decimal? LinhaCorteTrimestral { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("transactioncurrencyid")]
        public Lookup Moeda { get; set; }

        [LogicalAttribute("StateCode")]
        public int? Status { get; set; }
        
        #endregion
    }
}
