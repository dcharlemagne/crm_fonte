using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IQuestionarioPergunta<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarQuestionarioPerguntaPorQuestionario(string questionarioId);

        List<T> ListarQuestionarioPerguntaPorNomeQuestionario(string nomeQuestionario);

    }
}
