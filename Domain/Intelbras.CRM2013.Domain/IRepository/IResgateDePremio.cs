using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.ValueObjects;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IResgateDePremio<T> : IRepository<T>, IRepositoryBase
    {
        List<ResgateDoParticipante> ListarPor(Guid participanteId);
    }
}
