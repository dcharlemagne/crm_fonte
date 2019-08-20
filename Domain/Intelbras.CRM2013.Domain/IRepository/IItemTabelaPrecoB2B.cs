using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IItemTabelaPrecoB2B<T>:IRepository<T>,IRepositoryBase
    {
        T ObterPor(Guid? tabela, string codigoItemPreco);

        bool AlterarStatus(Guid id, int status);
    }
}
