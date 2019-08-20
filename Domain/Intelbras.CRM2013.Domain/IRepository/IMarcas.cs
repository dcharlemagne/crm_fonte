using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using System.Data;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IMarcas<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarMarcasPorConta(string contaId);
        T obterPorNome(string nome);        
        void associarMarcas(List<Marcas> lstMarcas, Guid contaid);
        void desassociarMarcas(List<Marcas> lstMarcas, Guid contaid);
    }
}
