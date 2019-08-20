using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class UtilService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public UtilService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public UtilService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        #endregion

        #region Métodos
        public bool MudarProprietarioRegistro(object objProprietario, Guid idProprietarioNovo, object entidadeDestino, Guid idEntidadeDestino)
        {
            return RepositoryService.Util.MudarProprietarioRegistro(objProprietario, idProprietarioNovo, entidadeDestino, idEntidadeDestino);
        }

        public bool MudarProprietarioRegistro(string tipoProprietario, Guid idProprietarioNovo, string entidadeDestino, Guid idEntidadeDestino)
        {
            return RepositoryService.Util.MudarProprietarioRegistro(tipoProprietario, idProprietarioNovo, entidadeDestino, idEntidadeDestino);
        }
        #endregion
    }
}
