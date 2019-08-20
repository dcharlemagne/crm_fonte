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
    public class StatusBeneficiosService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public StatusBeneficiosService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline); 
        }


        public StatusBeneficiosService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }

        #endregion

        public StatusBeneficios ObterPor(Guid idStatus)
        {
            return RepositoryService.StatusBeneficios.ObterPor(idStatus);
        }
    }
}
