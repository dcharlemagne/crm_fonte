using System;
using Intelbras.CRM2013.Domain.Model;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class RegiaoDeAtuacaoService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public RegiaoDeAtuacaoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }
        
        public RegiaoDeAtuacaoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        #endregion

        #region Métodos
        public string IntegracaoBarramento(RegiaoDeAtuacao objModelRegiaoDeAtuacao)
        {
            Domain.Integracao.MSG0297 msg = new Domain.Integracao.MSG0297(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            return msg.Enviar(objModelRegiaoDeAtuacao);
        }
        #endregion
    }
}