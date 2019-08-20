using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class MarcasServices
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public MarcasServices(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public MarcasServices(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos        
        public List<Marcas> ListarMarcasPorConta(string contaId)
        {
            return RepositoryService.Marcas.ListarMarcasPorConta(contaId);
        }
        public Marcas obterPorNome(string nome)
        {
            return RepositoryService.Marcas.obterPorNome(nome);
        }
        public void associarMarcas(List<Marcas> lstMarcas, Guid contaId)
        {
            RepositoryService.Marcas.associarMarcas(lstMarcas, contaId);
        }
        public void desassociarMarcas(List<Marcas> lstMarcas, Guid contaId)
        {
            RepositoryService.Marcas.desassociarMarcas(lstMarcas, contaId);
        }
        #endregion

    }
}
