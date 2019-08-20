using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class PaisServices
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public PaisServices(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public PaisServices(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public Pais BuscaPais(String Codigo)
        {
            List<Pais> lstConta = RepositoryService.Pais.ListarPor(Codigo);
            if (lstConta.Count > 0)
                return lstConta.First<Pais>();
            return null;
        }

        public Pais BuscaPais(Guid paisId)
        {
            return RepositoryService.Pais.ObterPor(paisId);
        }

        #endregion

    }
}
