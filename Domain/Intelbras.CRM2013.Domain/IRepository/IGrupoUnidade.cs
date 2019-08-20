using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IGrupoUnidade<T>:IRepository<T>,IRepositoryBase
    {
        List<T> ListarPor(Guid uomscheduleid);
        T ObterPor(Guid uomscheduleid);
        T ObterPor(string nomeGrupo);
    }
}
