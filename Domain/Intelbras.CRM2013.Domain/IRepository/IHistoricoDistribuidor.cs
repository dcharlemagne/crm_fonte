using SDKore.DomainModel;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IHistoricoDistribuidor<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPorRevendaComDataFim(Guid revendaId, DateTime? dataInicio = null, DateTime? dataFim = null);
        List<T> ListarPorRevendaSemDataFim(Guid revendaId, DateTime? dataInicio = null);
        Boolean AlterarStatus(Guid historicoid, int status);
        List<T> ListarPorPeriodo(DateTime data);
    }
}