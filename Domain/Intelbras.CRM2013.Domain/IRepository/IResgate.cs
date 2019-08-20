using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IResgate<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ObterPorRevenda(Guid revenda);
        List<T> ObterPorContato(Guid contatoId, params string[] columns);
        List<T> ObterPorResgateParceiro(Domain.Enum.Resgate.RazaoDoStatus statuscode, int count, int pageNumber, string cookie);
        void AlterarStatus(Guid id, int statuscode, bool stateActive);
    }
}
