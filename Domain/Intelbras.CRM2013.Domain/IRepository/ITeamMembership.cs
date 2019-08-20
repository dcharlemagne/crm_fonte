using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ITeamMembership<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid teamid);
        T ObterPor(Guid teamid);
    }
}
