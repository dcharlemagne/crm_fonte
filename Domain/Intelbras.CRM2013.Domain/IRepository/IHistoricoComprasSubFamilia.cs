using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IHistoricoComprasSubFamilia<T> : IRepository<T>, IRepositoryBase
    {
        DataTable ListarPor(string ano, string trimestre);
        T ObterPor(Guid UnidadeNeg, Guid subfamilia, Guid segmento, Guid familia,int ano,int trimestre);

    }
}
