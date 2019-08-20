using Intelbras.CRM2013.Domain.Model;
using Microsoft.Xrm.Sdk;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class SolicitacaoXUnidadesService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public SolicitacaoXUnidadesService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public SolicitacaoXUnidadesService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public SolicitacaoXUnidadesService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }
        #endregion

        public Guid Criar(Model.SolicitacaoXUnidades solicitacaoXUnidades)
        {
            return RepositoryService.SolicitacaoXUnidades.Create(solicitacaoXUnidades);
        }
        public void Deletar(Guid solicitacaoXUnidades)
        {
            RepositoryService.SolicitacaoXUnidades.Delete(solicitacaoXUnidades);
        }

        public void Atualizar(SolicitacaoXUnidades solicitacao)
        {
            RepositoryService.SolicitacaoXUnidades.Update(solicitacao);
        }
                
        public SolicitacaoXUnidades ObterPor(Guid solicitacaoId)
        {
            return RepositoryService.SolicitacaoXUnidades.Retrieve(solicitacaoId);
        }

        public List<SolicitacaoXUnidades> ListarPor(Guid id)
        {
            return RepositoryService.SolicitacaoXUnidades.ListarPor(id);
        }
    }
}
