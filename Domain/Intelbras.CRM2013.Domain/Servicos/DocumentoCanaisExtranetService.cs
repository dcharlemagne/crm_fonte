using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class DocumentoCanaisExtranetService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public DocumentoCanaisExtranetService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public DocumentoCanaisExtranetService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public DocumentoCanaisExtranetService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        #region Métodos

        public List<DocumentoCanaisExtranet> ListarDocumentosDoCanalDaExtranet(Guid[] classificacoesId, Guid[] categoriasId, Enum.DocumentoCanaisExtranet.RazaoStatus razaoStatus, bool somenteVigente)
        {
            return RepositoryService.DocumentoCanaisExtranet.ListarPor(classificacoesId, categoriasId, razaoStatus, somenteVigente);
        }

        public List<DocumentoCanaisExtranet> ListarParaTodosCanais(Enum.DocumentoCanaisExtranet.RazaoStatus razaoStatus, bool somenteVigente)
        {
            return RepositoryService.DocumentoCanaisExtranet.ListarPor(new Guid[0], new Guid[0], razaoStatus, somenteVigente, true);
        }

        #endregion

    }
}
