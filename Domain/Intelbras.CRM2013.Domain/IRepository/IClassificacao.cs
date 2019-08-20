using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IClassificacao<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(int? stateCode, bool? participaPrograma);
        T ObterPor(Guid classificacaoId);
        T ObterPor(String nome);
        bool AlterarStatus(Guid id, int status);
    }
}
