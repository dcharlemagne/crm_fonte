using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_subfamiliadeproduto")]
    public class SubfamiliaProduto : IntegracaoBase
    {
        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public SubfamiliaProduto() { }

        public SubfamiliaProduto(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public SubfamiliaProduto(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_subfamiliadeprodutoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_codigo_subfamilia")]
        public String Codigo { get; set; }

        [LogicalAttribute("itbc_famildeprod")]
        public Lookup Familia { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("statecode")]
        public new int? Status { get; set; }
        #endregion
    }
}
