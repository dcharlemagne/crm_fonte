using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IParametroBeneficio<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(Guid beneficioId);
        List<T> ListarPor(Guid beneficioId);
    }
}
