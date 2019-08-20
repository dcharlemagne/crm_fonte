using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ICompromissosDoPrograma<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(string NomeCompromisso);
        T ObterPor(int codigo);
        T ObterPor(Guid CompromissosDoProgramaId);
        T ObterPorCompCanal(Guid CompromissosDoCanalId);
    }
}
