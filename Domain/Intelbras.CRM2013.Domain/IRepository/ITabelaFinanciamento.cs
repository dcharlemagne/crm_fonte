using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ITabelaFinanciamento<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid itbc_tabeladefinanciamentoid);
        T ObterPor(string nome);
        List<T> ListarPor(string nome);
        bool AlterarStatus(Guid TabelaFinanciamento,int statecode, int statuscode);

    }
}
