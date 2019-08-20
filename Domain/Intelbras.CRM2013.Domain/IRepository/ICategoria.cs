using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ICategoria<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(int statecode);
        T ObterPor(Guid categoriaId);
        T ObterPor(string codigoCategoria);
        T ObterPorNome(string nome);
    }
}
