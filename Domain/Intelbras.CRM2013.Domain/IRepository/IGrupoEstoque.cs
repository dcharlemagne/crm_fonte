using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IGrupoEstoque<T>:IRepository<T>,IRepositoryBase
    {
        T ObterPor(int codigoGrupoEstoque);
        T ObterPor(Guid grupoEstoqueId);
        List<T> ListarPor(string[] codigo);
        bool AlterarStatus(Guid SubFamiliaID, int statecode, int statuscode);

    }
}
