using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class SubClassificacaoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public SubClassificacaoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline); 
        }

        public SubClassificacaoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }

        #endregion

        #region Métodos

        public string IntegracaoBarramento(Subclassificacoes subclassificacao)
        {
            Domain.Integracao.MSG0016 msgSubclassificacao = new Domain.Integracao.MSG0016(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            return msgSubclassificacao.Enviar(subclassificacao);
        }

        public Subclassificacoes ObterSubClassificacaoPorNome(String nome)
        {
            return RepositoryService.Subclassificacoes.ObterPor(nome);
        }

        #endregion
    }
}