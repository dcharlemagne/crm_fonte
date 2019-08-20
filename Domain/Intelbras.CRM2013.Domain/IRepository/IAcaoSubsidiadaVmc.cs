using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IAcaoSubsidiadaVmc<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(Guid codigoId);
        List<T> ListarPor(Guid codigoId);
        List<T> Listar();
        T ObterPorCodigo(String codigoAcao);
    }
}
