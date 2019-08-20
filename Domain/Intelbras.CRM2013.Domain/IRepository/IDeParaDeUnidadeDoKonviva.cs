using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IDeParaDeUnidadeDoKonviva<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Dictionary<string, object> campos, DateTime? datainicial, bool somenteAtivos = true);
    }
}
