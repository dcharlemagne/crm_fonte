using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_receitapadrao")]
    public class ReceitaPadrao : IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ReceitaPadrao() { }

        public ReceitaPadrao(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public ReceitaPadrao(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_receitapadraoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_codigoreceitapadrao")]
        public int? CodReceitaPadrao { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("statecode")]
        public int? State { get; set; }

        #endregion
    }
}
