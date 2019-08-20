using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using System.Data;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ISinonimosMarcas<T> : IRepository<T>, IRepositoryBase
    {
        T obterPorNome(string nome);
        void associarSinonimosMarcas(List<SinonimosMarcas> lstSinonimosMarcas, Guid contaid);
    }
}
