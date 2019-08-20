using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class HistoricoCategoriaService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public HistoricoCategoriaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public HistoricoCategoriaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public HistoricoCategoriaService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        #region Métodos

        public HistoricoCategoria Persistir(HistoricoCategoria objHistorico)
        {
            HistoricoCategoria TmpHistorico = null;
            if (objHistorico.ID.HasValue)
                TmpHistorico = ObterPor(objHistorico.ID.Value);

            if (TmpHistorico != null)
            {
                RepositoryService.HistoricoCategoria.Update(objHistorico);
                return TmpHistorico;
            }
            else
                objHistorico.ID = RepositoryService.HistoricoCategoria.Create(objHistorico);
            return objHistorico;
        }

        public HistoricoCategoria ObterPor(Guid historicoId, params string[] columns)
        {
            return RepositoryService.HistoricoCategoria.Retrieve(historicoId, columns);
        }


        #endregion
    }
}
