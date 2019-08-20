using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IFormaPagamento<T> : IRepository<T>, IRepositoryBase
    {
        List<T> Listar();
        T ObterPor(string nomeFormaPagamento);
    }
}
