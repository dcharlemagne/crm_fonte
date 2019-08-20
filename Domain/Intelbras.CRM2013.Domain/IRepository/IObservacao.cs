using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IObservacao<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPorParametrosGlobais(Guid objectid);
        T Obter(string Entidade, string LinkTo, string LinkFrom, string FieldFilter, Guid ID);
        T ObterPorMeta(Guid metaid);
    }
}