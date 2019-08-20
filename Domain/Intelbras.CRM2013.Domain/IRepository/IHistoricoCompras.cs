using SDKore.DomainModel;
using System;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IHistoricoCompras<T> : IRepository<T>, IRepositoryBase
    {
        DataTable ListarPor(string ano);
        T ObterPor(Guid UnidadeNeg, Int32 ano, params string[] columns);
    }
}