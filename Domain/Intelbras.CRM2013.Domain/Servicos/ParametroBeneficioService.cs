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
    public class ParametroBeneficioService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ParametroBeneficioService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public ParametroBeneficioService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos
       

        public List<ParametroBeneficio> ListarPorBeneficio(Guid beneficioId)
        {
            return RepositoryService.ParametroBeneficio.ListarPor(beneficioId);
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.Bens.AlterarStatus(id, status);
        }

        #endregion
    }
}
