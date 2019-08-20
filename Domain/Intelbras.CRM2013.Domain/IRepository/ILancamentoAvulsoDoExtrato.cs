using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ILancamentoAvulsoDoExtrato<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Extrato extrato);
        List<T> ListarSemExtratoPor(Model.Conta autorizada);
    }
}
