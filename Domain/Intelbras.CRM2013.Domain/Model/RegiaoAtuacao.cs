using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_account_itbc_municipios")]
    public class RegiaoAtuacao  : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public RegiaoAtuacao() { }

        public RegiaoAtuacao(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public RegiaoAtuacao(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        //[LogicalAttribute("itbc_account_itbc_municipiosId")]
        //public Guid? ID { get; set; }

        [LogicalAttribute("accountid")]
        public Guid? Canal { get; set; }

        [LogicalAttribute("itbc_municipiosid")]
        public Guid? MunicipioId { get; set; }

        #endregion
    }
}
