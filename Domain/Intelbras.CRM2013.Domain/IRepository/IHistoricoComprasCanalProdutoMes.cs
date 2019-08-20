using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IHistoricoComprasCanalProdutoMes<T> : IRepository<T>, IRepositoryBase
    {
        DataTable ListarPor(string ano, string trimestre);
        T ObterPor(Int32 trimestre, Int32 mes, Int32 ano, Guid produto, Guid canal);
    }
}
