using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IIntegrador<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(string[] atributoConsulta, string[] valorConsulta);
    }
}
