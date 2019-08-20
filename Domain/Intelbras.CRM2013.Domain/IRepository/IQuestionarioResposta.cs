using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IQuestionarioResposta<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarTodos();
        T ObterPorOpcaoId(Guid opcaoId, string contaId, bool status);
        bool AlterarStatus(Guid opcaoIdquestinarioId, int status);
        List<T> ObterRespostasConta(string contaId, bool status);
    }

}
