using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IFila<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(Guid queueid);
        T ObterPor(String nome);
        T ObterFilaPublicaPor(Guid usuarioId);
    }
}
