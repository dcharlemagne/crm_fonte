
using System;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class TipodeParametroGlobalService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public TipodeParametroGlobalService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public TipodeParametroGlobalService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Metodos

        public TipodeParametroGlobal ObterPor(int codigo)
        {
            return RepositoryService.TipodeParametroGlobal.ObterpoCodigo(codigo);
        }

        public TipodeParametroGlobal ObterPor(Guid idTipoParam)
        {
            return RepositoryService.TipodeParametroGlobal.ObterPor(idTipoParam);
        }

        public TipodeParametroGlobal ObterPor(string nomeTipoParametro)
        {
            return RepositoryService.TipodeParametroGlobal.ObterPorNomeParametro(nomeTipoParametro);
        }

        #endregion


    }
}

