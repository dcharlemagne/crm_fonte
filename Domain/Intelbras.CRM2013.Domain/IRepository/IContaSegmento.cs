using SDKore.DomainModel;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IContaSegmento<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid id);
    }
}
