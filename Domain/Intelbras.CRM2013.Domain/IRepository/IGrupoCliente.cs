using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IGrupoCliente<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(Guid contaId);
        List<T> ListarTodos();
    }
}
