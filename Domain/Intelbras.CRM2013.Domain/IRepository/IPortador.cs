using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPortador<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid accountid);
        List<T> ListarPor(Int32 codigoportador);
        bool AlterarStatus(Guid itbc_portadorid, int status,int stateCode);
        T ObterPor(Guid accountid);
        T ObterPor(int condigoPortador);
    }
}
