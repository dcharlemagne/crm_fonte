using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IHistoricoComprasSegmento<T> : IRepository<T>, IRepositoryBase
    {
        DataTable ListarPor(string ano, string trimestre);
        T ObterPor(Guid UnidadeNeg,Guid Segmento, Int32 ano, String Trimestre);
    }
}
