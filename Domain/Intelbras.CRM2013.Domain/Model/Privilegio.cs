using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("privilege")]
    public class Privilegio : DomainBase
    {
        #region Construtor
        private RepositoryService RepositoryService { get; set; }

        public Privilegio() { }

        public Privilegio(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Privilegio(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Metodos
        public List<Privilegio> PesquisarPrivilegioPor(Usuario usuario)
        {
            return RepositoryService.Privilegio.PesquisarPrivilegioPor(usuario);
        }

        public Boolean PermissaoIncluirEm(Usuario usuario, String entidade)
        {
            return RepositoryService.Privilegio.PermissaoIncluirEm(usuario, entidade);
        }
        #endregion
    }
}
