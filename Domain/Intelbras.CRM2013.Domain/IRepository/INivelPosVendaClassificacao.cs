using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface INivelPosVendaClassificacao<T>:IRepository<T>,IRepositoryBase
    {
        List<T> ListarPor(Guid itbc_posvendaid);
        T ObterPor(Guid itbc_posvendaid);
    }
}
