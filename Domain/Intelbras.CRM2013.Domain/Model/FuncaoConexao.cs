using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("connectionrole")]
    public class FuncaoConexao : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public FuncaoConexao() { }

        public FuncaoConexao(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public FuncaoConexao(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("connectionroleid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("name")]
        public string Nome { get; set; }

        [LogicalAttribute("description")]
        public string Descricao { get; set; }

        [LogicalAttribute("statuscode")]
        public int? Status { get; set; }
        
        [LogicalAttribute("statecode")]
        public int? State { get; set; }

        [LogicalAttribute("category")]
        public int? Categoria { get; set; }

        #endregion
    }
}
