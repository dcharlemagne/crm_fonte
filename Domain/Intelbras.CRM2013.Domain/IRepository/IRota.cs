using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IRota<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid id);
        T ObterPor(String codigo);
        T ObterPor(Guid id);
        bool AlterarStatus(Guid Id, int status);
    }
}
