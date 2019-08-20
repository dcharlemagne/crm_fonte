using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ISharePointDocumentLocation<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(Guid regardingobjectid);
    }
}
