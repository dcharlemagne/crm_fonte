using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IAdvertencia<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Model.Conta cliente);
    }
}
