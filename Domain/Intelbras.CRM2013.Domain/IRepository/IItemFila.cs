using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IItemFila<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(Guid queueid);       
    }
}
