using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IBens<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid itbc_imoveisdaempresaid);
        T ObterPor(Guid itbc_imoveisdaempresaid);
        bool AlterarStatus(Guid itbc_imoveisdaempresaid, int status);
    }
}
