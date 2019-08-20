using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ITipoDeAtividade<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid TipoDeAtividade);
        List<T> ListarPorNome(string NomeAtividade);
        T ObterPor(Guid TipoDeAtividade);
        T ObterPorPapel(Guid PapelId);
    }
}
