using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;
namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IFuncaoRelacionamento<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ObterPor(Model.Conta cliente, Guid funcao);
        Guid ObterFuncaoPor(string nome);
        List<T> ListarTodas();
    }
}