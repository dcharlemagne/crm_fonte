using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ITabelaPreco<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid id);
        List<TabelaDePreco> ListarPor(Domain.Model.Conta cliente);
        T ObterPor(int codigoTabela);
        T ObterPor(Guid id);
        bool AlterarStatus(Guid id, int status);
    }
}
