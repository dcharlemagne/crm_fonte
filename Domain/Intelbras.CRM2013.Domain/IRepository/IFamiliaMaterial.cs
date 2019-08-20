using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IFamiliaMaterial<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid familiaMaterialId);
        T ObterPor(Guid familiaMaterialId);
        T ObterPor(string codigoFamiliaMaterial);
        bool AlterarStatus(Guid familiaMaterialId,int statuscode);
    }
}
