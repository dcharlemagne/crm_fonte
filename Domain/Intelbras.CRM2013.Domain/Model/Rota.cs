using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_rota")]
    public class Rota : IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Rota() { }

        public Rota(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Rota(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("itbc_rotaid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_codigo_rota")]
        public String CodigoRota { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_roteiro")]
        public String Roteiro { get; set; }

        [LogicalAttribute("statecode")]
        public int? State { get; set; }
        #endregion
    }
}
