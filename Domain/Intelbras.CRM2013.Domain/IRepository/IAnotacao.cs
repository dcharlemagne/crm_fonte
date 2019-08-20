using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IAnotacao<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid objectId);
        List<T> ListarPor(Guid objectId, bool comAnexo);
        List<T> ListarPorTipoArquivo(string objectId, string tipoArquivo);
    }
}
