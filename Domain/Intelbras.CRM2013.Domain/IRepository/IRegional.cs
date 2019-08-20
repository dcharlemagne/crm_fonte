using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IRegional<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Usuario usuario);
        T ObterPor(Municipio cidade);
    }
}