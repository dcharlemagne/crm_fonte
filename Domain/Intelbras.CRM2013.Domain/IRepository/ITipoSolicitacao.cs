using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ITipoSolicitacao<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor();
        T ObterPor(Guid tipoSolicitacaoId);
    }
}
