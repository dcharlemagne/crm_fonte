using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IConexao<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid? funcaoAte, Guid? conectadoA, Guid? funcaoDe, Guid? conectadoDe);
        T ObterPor(Guid conexaoId);
        void AlterarStatus(Guid conexaoId, int state, int status, Guid usuarioId);
        Dictionary<int, string> GetOptionSetValues(string entidade, string campo);
    }
}
