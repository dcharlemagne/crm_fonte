using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IOrigem<T>:IRepository<T>,IRepositoryBase
    {
        List<T> ListarPor(Guid itbc_origemid);
        T ObterPor(Guid itbc_origemid);
        List<T> ListarPor(String codigoOrigem);
        bool AlterarStatus(Guid itbc_origemid, int status);
    }
}
