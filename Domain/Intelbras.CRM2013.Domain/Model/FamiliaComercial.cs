using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_familiacomercial")]
    public class FamiliaComercial : IntegracaoBase
    {
        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public FamiliaComercial() { }

        public FamiliaComercial(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public FamiliaComercial(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }


        #endregion

        #region Atributos
        [LogicalAttribute("itbc_familiacomercialid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_businessunit")]
        public Lookup UnidadeNegocios { get; set; }

        [LogicalAttribute("itbc_codigo_familia_comercial")]
        public String Codigo { get; set; }

        [LogicalAttribute("itbc_famildeprod")]
        public Lookup Familia { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_origem")]
        public Lookup Origem { get; set; }

        [LogicalAttribute("itbc_segmento")]
        public Lookup Segmento { get; set; }

        [LogicalAttribute("itbc_subfamiliadeproduto")]
        public Lookup Subfamilia { get; set; }

        #endregion
    }
}
