using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface INivelPosVenda<T>:IRepository<T>,IRepositoryBase
    {
        List<T> ListarPor(Guid itbc_posVenda);
        T ObterPor(Guid itbc_posVenda);
    }
}
