using System;
using System.Collections.Generic;
using System.Data;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IProdutoProjeto<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPorClientePotencial(Guid clientePotencialId);
    }
}
