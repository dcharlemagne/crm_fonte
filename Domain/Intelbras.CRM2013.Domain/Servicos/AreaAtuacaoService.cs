using System;
using Intelbras.CRM2013.Domain.Model;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class AreaAtuacaoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public AreaAtuacaoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public AreaAtuacaoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos
        public AreaAtuacao ObterPor(Guid areaAtuacaoId)
        {
            return RepositoryService.AreaAtuacao.ObterPor(areaAtuacaoId);
        }

        public AreaAtuacao ObterPorCodigo(int codigoAreaAtuacao)
        {
            return RepositoryService.AreaAtuacao.ObterPorCodigo(codigoAreaAtuacao);
        }

        public List<AreaAtuacao> ListarPorContato(Guid contatoId)
        {
            return RepositoryService.AreaAtuacao.ListarPorContato(contatoId);
        }

        #endregion
    }
}