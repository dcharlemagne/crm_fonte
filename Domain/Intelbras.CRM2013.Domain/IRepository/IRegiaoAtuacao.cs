using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IRegiaoAtuacao<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid accountId);
        T ObterPor(Guid municipioId, Guid accountId);
        //T ObterPor(string nome);
    }
}
