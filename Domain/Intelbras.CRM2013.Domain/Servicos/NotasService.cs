using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class NotasService
    {
        private RepositoryService RepositoryService { get; set; }

        public NotasService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public NotasService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public List<Anotacao> ListarAnotacoesPor(Guid objectId)
        {
            return RepositoryService.Anexo.ListarPor(objectId);
        }

        public List<Anotacao> ListarAnotacoesPorTipoArquivo(string objectId, string tipoArquivo)
        {
            return RepositoryService.Anexo.ListarPorTipoArquivo(objectId, tipoArquivo);
        }
    }
}
