using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IBeneficiosCompromissos<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid PerfilId);
        List<T> ListarPor(Guid PerfilId,Guid? CompromissoId,Guid? BeneficioId);
        T ObterPor(Guid BeneficiosCompromissosID);
    }
}
