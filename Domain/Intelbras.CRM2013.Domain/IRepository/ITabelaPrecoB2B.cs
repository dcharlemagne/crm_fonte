using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ITabelaPrecoB2B<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid id);
        T ObterPor(string codigoTabela);
        bool AlterarStatus(Guid id, int status);
    }
}
