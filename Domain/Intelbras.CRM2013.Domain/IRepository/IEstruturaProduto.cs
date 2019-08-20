using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IEstruturaProduto<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid accountid);
        T ObterPor(Guid accountid);
    }
}
