using SDKore.DomainModel;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IRelacionamentoCliente<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarParticipantePor(Guid participanteId, Guid funcaoRelacionamentoId);
        List<T> ListarParticipantePorTopUm(Guid participanteId, Guid funcaoRelacionamentoId);
        List<T> ObterPor(Guid participanteUmId, Guid funcaoRelacionamentoUmId, Guid participanteDoisId, Guid funcaoRelacionamentoDoisId);
        List<T> ListarParticipantePor(Guid participanteId, Guid funcaoRelacionamentoId, Guid funcaoparceiroId);
    }
}
