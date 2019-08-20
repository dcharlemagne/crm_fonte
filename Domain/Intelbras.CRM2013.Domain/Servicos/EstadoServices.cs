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
    class EstadoServices
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public EstadoServices(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public EstadoServices(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public Estado BuscaEstado(String ChaveIntegracao)
        {
            List<Estado> lstEstado = RepositoryService.Estado.ListarPor(ChaveIntegracao);
            if (lstEstado.Count > 0)
                return lstEstado.First<Estado>();
            return null;
        }

        public Estado BuscaEstadoPorSigla(String siglaUF)
        {
            List<Estado> lstEstado = RepositoryService.Estado.ListarPorSigla(siglaUF);
            if (lstEstado.Count > 0)
                return lstEstado.First<Estado>();
            return null;
        }
        public Estado BuscaEstadoPorNome(String nome)
        {
            return RepositoryService.Estado.ObterPor(nome);
        }

        public Estado BuscaEstadoPorId(Guid estadoId)
        {
            return RepositoryService.Estado.ObterPor(estadoId);
        }
        #endregion  
    }
}
