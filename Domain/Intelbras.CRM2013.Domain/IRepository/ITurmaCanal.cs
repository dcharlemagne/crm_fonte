using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ITurmaCanal<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid turmaId);
        T ObterPor(Guid turmaId);
        Boolean AlterarStatus(Guid treinamentoId, int status);
        T ObterPorIdTurma(String identificadorTurma);
    }
}
