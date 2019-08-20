
using System;
using System.Collections.Generic;
using SDKore.DomainModel;
namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ITipodeParametroGlobal<T> : IRepository<T>, IRepositoryBase
    {
        T ObterpoCodigo(int codigo);
        T ObterPorNomeParametro(string nomeTipoParametro);

        T ObterPor(Guid id);
    }
}

