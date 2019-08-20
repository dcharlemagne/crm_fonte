using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_posvenda")]
    public class NivelPosVenda:DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public NivelPosVenda() { }

        public NivelPosVenda(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public NivelPosVenda(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
            [LogicalAttribute("itbc_posvendaid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("itbc_name")]
            public string Nome { get; set; }

            [LogicalAttribute("statuscode")]
            public int? Status { get; set; }

            [LogicalAttribute("statecode")]
            public int? State { get; set; }
        #endregion
    }
}
