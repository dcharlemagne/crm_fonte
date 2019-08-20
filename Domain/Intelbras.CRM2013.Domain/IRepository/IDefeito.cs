using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IDefeito<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(LinhaComercial linhaComercial);
        List<T> ListarPor(Product produto);
    }
}
