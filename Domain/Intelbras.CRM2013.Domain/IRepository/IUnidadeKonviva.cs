using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IUnidadeKonviva<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPorIdInterno(Int32 idInterno);
        T ObterPorNome(String nome);
        T ObterPor(Guid idUnidadeKonviva);
    }
}
