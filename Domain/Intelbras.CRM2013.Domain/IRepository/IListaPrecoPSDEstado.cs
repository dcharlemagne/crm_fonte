using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IListaPrecoPSDEstado<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid? estado, Guid? listaPSD);
    }
}
