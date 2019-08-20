using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IBeneficio<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(Guid beneficioId);
        T ObterPor(int codigo);
        T ObterPorBenCanalId(Guid beneficioCanalId);
        List<T> ListarPor(Guid beneficioId);

    }
}
