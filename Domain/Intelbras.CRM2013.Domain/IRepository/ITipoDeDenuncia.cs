using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ITipoDeDenuncia<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(Guid TipoDeAtividade);
        List<T> Listar();
    }
}
