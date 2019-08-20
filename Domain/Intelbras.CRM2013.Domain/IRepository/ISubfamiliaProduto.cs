using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ISubfamiliaProduto<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid subfamiliaid);
        T ObterPor(string codSubfamilia);
        T ObterPor(Guid subfamiliaid);
        List<T> ListarPor(String accountid);
        bool AlterarStatus(Guid SubFamiliaID, int statecode, int statuscode);

        List<T> ListaPorUnidadeNegocio(Guid unidadenegocioId);
    }
}
