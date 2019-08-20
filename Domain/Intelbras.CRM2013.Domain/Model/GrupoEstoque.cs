using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_grupodeestoque")]
    public class GrupoEstoque : IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public GrupoEstoque() { }

        public GrupoEstoque(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public GrupoEstoque(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_grupodeestoqueid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_codigo_grupo_estoque")]
        public Int32? Codigo { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }

        [LogicalAttribute("StateCode")]
        public int? Status { get; set; }
        #endregion
    }
}
