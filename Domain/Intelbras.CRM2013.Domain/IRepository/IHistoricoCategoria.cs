using System;
using System.Collections.Generic;
using System.Data;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IHistoricoCategoria<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPorConta(Guid contaId);
        T ObterPor(Guid historicoId);

    }
}
