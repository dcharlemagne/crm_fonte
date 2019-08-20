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
    public class CnaeService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public CnaeService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public CnaeService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos
        
        public CNAE ObterPor(string classe)
        {
            return RepositoryService.CNAE.ObterPor(classe);
        }

        public CNAE ObterPor(Guid id)
        {
            return RepositoryService.CNAE.Retrieve(id);
        }

        public void Update(CNAE cnae)
        {
            RepositoryService.CNAE.Update(cnae);
        }

        public void Integrar(CNAE cnae)
        {
            var mensagem = new Domain.Integracao.MSG0182(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            mensagem.Enviar(cnae);
        }

        #endregion
    }
}