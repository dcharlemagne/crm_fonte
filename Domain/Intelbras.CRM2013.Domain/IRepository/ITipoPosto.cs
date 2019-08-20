using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ITipoPosto<T> : IRepository<T>, IRepositoryBase
    {
        decimal ObterMaiorValorPor(Model.Conta cliente);
        List<T> ListarPor(string login);
    }
}
