using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ILinhaComercial<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(Product produto);
        T ObterLinhaComercialPor(FamiliaComercial familiaComercial);
        List<T> ListarLinhaComercialPor(FamiliaComercial familiaComercial);
    }
}
