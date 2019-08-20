using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class GrupoUnidadeMedidaService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public GrupoUnidadeMedidaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public GrupoUnidadeMedidaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public GrupoUnidade ObterPor(string nomeGrupo)
        {
            return RepositoryService.GrupoUnidadeMedida.ObterPor(nomeGrupo);
        }

        public GrupoUnidade ObterPor(Guid grupoUnidadeId)
        {
            return RepositoryService.GrupoUnidadeMedida.ObterPor(grupoUnidadeId);
        }

        #endregion

    }
}