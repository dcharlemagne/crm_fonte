using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ILinhaCorteRevenda<T>:IRepository<T>,IRepositoryBase
    {
        List<T> ListarPor(Guid itbc_linhadecorterevendaid);
        T ObterPor(Guid itbc_linhadecorterevendaid);
        List<T> ListarPor(List<Guid> itbc_businessunitid,Guid? CategoriaLinha);        
    }
}
