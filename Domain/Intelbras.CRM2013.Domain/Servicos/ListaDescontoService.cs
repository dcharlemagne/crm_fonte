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
    public class ListaDescontoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ListaDescontoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public ListaDescontoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        public ListaDesconto BuscaListaDesconto(String nomeListaDesconto)
        {
            List<ListaDesconto> lstnomeListaDesconto = RepositoryService.ListaDesconto.ListarPor(nomeListaDesconto);
            if (lstnomeListaDesconto.Count > 0)
                return lstnomeListaDesconto.First<ListaDesconto>();
            return null;
        }
        
    }
}
