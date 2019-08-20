using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_marca_equipamento")]
    public class Marca : DomainBase
    {
        #region Construtor

        private RepositoryService RepositoryService { get; set; }

        public Marca() { }

        public Marca(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Marca(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("itbc_marca_equipamentoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_apresenta_portal_operacoes_suporte")]
        public Boolean? ApresentaPortalOperacoeseSuporte { get; set; }

        #endregion

        #region Metodos

        #endregion
    }
}