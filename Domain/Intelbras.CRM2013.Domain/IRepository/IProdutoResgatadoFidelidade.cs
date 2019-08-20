using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IProdutoResgatadoFidelidade<T> : IRepository<T>, IRepositoryBase
    {
        void AlterarStatus(Guid id, int statuscode, bool stateActive);
        List<T> ListarAtivosPor(Model.Resgate resgate, params string[] columns);
    }
}
