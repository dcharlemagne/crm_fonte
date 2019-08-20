using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class SinonimosMarcasServices
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public SinonimosMarcasServices(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public SinonimosMarcasServices(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos
        public SinonimosMarcas obterPorNome(string nome)
        {
            return RepositoryService.SinonimosMarcas.obterPorNome(nome);
        }
        public void associarSinonimoMarcas(List<SinonimosMarcas> lstSinonimosMarcas, Guid contaId)
        {
            RepositoryService.SinonimosMarcas.associarSinonimosMarcas(lstSinonimosMarcas, contaId);
        }
        #endregion

    }
}
