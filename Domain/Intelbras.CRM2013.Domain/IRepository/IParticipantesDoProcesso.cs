using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IParticipantesDoProcesso<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid Processo, int Ordem);
        T ObterPor(Guid ParticipantesDoProcessoId);
        T ObterPor(int ordem, Guid tipoSolicitacao);
    }
}
