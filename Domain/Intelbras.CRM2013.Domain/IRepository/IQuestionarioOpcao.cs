using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IQuestionarioOpcao<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarQuestionarioOpcaoPor(Guid questionarioPerguntaId);
        List<T> ListarPorContaId(Guid contaId);
    }
}
