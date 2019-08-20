using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ISeguroConta<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid itbc_segurodacontaid);
        T ObterPor(Guid itbc_segurodacontaid);
        bool AlterarStatus(Guid itbc_estadoid, int status);
    }
}
