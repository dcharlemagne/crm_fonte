using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IFuncaoConexao<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(int? categoria);
        List<T> ListarPorNome(String nome);
    }
}
