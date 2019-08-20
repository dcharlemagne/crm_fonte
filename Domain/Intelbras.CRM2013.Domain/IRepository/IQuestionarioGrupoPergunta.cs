using SDKore.DomainModel;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IQuestionarioGrupoPergunta<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarGrupoById(string questionarioId);
    }
}
