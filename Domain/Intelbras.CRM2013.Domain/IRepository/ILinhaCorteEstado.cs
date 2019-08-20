using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ILinhaCorteEstado<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid linhaCorteDistId, Guid? estadoId);
        List<T> ListarPor(Guid estadoId);
        T ObterPor(Guid linhaCorteDistId);
    }
}
