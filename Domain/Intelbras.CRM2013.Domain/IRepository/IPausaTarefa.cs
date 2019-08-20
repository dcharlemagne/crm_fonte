using SDKore.DomainModel;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPausaTarefa<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid activityid);
        List<T> ListarTarefaPor(Guid activityid);
    }
}