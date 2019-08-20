using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IListaDesconto<T>:IRepository<T>,IRepositoryBase
    {
        List<T> ListarPor(Guid discounttypeid);
        T ObterPor(Guid discounttypeid);
        List<T> ListarPor(String nomeListaDesconto);
    }
}
