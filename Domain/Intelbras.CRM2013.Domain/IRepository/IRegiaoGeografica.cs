using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IRegiaoGeografica<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(string nome);
        List<T> ListarPor(Guid itbc_regiaogeoId);
        T ObterPor(Guid accountid);
        bool AlterarStatus(Guid itbc_regiaogeoid, int status);
    }
}
