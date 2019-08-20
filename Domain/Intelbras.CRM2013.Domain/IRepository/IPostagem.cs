using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPostagem<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPorReferenteA(Guid referenteA);

    }
}
