using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ICategoriaB2B<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(int statecode);
        T ObterPor(Guid guid);
        T ObterPor(int codigoCategoriaB2B);
        void AlterarStatus(Guid guid, int state, int status);
    }
}
