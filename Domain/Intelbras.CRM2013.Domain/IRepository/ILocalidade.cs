using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ILocalidade<T> : IRepository<T>, IRepositoryBase
    {
        Localidade PesquisarEnderecoPor(string cep);
    }
}
