using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ITreinamentoCertificacao<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid treinamentoId);
        T ObterPor(Guid treinamentoId);
        Boolean AlterarStatus(Guid treinamentoId, int status);
        T ObterPor(Int32 idCurso);
       
    }
}
