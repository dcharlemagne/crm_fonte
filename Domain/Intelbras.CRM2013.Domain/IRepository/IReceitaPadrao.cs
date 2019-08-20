using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IReceitaPadrao<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor();
        T ObterPor(int codigoReceita);
        T ObterPor(Guid receitaPadraoId);
        void AlterarStatus(Guid receitaPadraoId, int state, int status, Guid usuarioId);
    }
}
