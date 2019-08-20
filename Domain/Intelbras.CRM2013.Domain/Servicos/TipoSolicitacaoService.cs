using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class TipoSolicitacaoService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public TipoSolicitacaoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public TipoSolicitacaoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public List<TipoSolicitacao> Listar()
        {
            return RepositoryService.TipoSolicitacao.ListarPor();
        }

        public TipoSolicitacao BuscaPor(Guid tipoSolicitacaoId)
        {
            return RepositoryService.TipoSolicitacao.ObterPor(tipoSolicitacaoId);
        }

        #endregion

    }
}