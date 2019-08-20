using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ILiveChatTracking<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPorOcorrenciaReferenciada(Guid ocorrenciaId);
    }
}
