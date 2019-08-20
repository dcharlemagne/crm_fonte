using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IItemListaPreco<T>:IRepository<T>,IRepositoryBase
    {
        List<T> ListarPor(Guid listaPrecoId, Guid? productid);
        T ObterPor(Guid productpricelevelid);
    }
}
