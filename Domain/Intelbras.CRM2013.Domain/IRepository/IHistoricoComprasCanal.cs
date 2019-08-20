using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IHistoricoComprasCanal<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(Guid UnidadeNeg, Int32 trimestre, Int32 ano, Guid canal);
        DataTable ListarPor(string ano, string trimestre);

    }
}
