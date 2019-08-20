using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IListaPrecoXEstado<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid? estadoId, Guid? listaId);
        T ObterPor(Guid listaPrecoXEstadoId);
    }
}
