using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IProdutoAssisteciaTecnica<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ObterPor(Guid produto, Guid assistenciaTecnica);
    }
}
