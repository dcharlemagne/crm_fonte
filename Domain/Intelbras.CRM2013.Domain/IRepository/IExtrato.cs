using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IExtrato<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(string numeroExtrato);
        List<T> ListarPor(DateTime dataCriacao);
        List<T> ListarPor(Model.Conta postoDeServico);
    }
}
