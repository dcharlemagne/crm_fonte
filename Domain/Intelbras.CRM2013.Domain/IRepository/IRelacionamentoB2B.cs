using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IRelacionamentoB2B<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid? RelacionamentoB2BID);
        T ObterPor(string codigoRelacionamentoB2B);
        T ObterPor(Guid guidCanal, Guid guidSupervisor, Guid guidKeyAccount, Guid guidAssistente, DateTime dtInicial, DateTime dtFinal);
        Boolean AlterarStatus(Guid id, int stateCode, int statusCode);
    }
}
