using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ILogisticaReversa<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarLogisticaReversaDo(Domain.Model.Conta postoDeServico);
    }
}
