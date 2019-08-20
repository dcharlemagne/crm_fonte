using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IFamiliaPoliticaComercial<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid politicaComercialId);
        List<T> ListarTodas();
    }
}
