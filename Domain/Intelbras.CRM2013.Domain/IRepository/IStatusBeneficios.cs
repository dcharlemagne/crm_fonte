using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IStatusBeneficios<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(string statusCompromissosNome);
        T ObterPor(Guid statusCompromissosId);
    }
}
