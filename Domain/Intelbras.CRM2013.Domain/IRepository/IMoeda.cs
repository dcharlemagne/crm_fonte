using System;
using System.Collections.Generic;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IMoeda<T>:IRepository<T>,IRepositoryBase
    {
        List<T> ListarPor(Guid transactioncurrencyid);
        T ObterPor(Guid transactioncurrencyid);
        T ObterPor(string isocurrencycode);
        List<T> ListarPor(String nomeMoeda);
    }
}
