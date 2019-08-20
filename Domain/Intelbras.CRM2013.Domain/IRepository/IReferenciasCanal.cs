using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IReferenciasCanal<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid itbc_referenciasdocanalid);
        T ObterPor(Guid itbc_referenciasdocanalid);
        bool AlterarStatus(Guid itbc_estadoid, int status);

    }
}
