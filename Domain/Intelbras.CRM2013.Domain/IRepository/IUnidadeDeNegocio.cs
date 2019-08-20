using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IUnidadeDeNegocio<T> : IRepository<T>, IRepositoryBase
    {
        UnidadeNegocio ObterPor(LinhaComercial linhaComercial);
        UnidadeNegocio ObterPor(Product produto);
    }
}