using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IUnidadeDeMedida<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid accountid);
        T ObterPor(Guid accountid);
        List<T> ListarPor(String nome);
        List<T> ListarPor(String[] nome);
        List<T> ListarTodos();
    }
}
