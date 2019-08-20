using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ISolucao<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarSolucaoesPor(string defeitoId);
        List<T> ListarSolucaoesPorFamilia(Guid linhaComercialId, Defeito defeito);
    }
}
