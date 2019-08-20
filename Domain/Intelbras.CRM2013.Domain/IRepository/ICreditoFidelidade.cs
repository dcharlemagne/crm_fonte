using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ICreditoFidelidade<T> : IRepository<T>, IRepositoryBase
    {
        bool PossuiPor(Guid participanteId, int pontosResgate, DateTime data);
        List<T> ListarDisponiveisPor(Guid participanteId, DateTime data);
        int QuantidadeCreditosAVencerPor(Guid participanteId, DateTime data);
        int QuantidadeCreditosDisponiveisPor(Guid participanteId, DateTime data);
        int QuantidadeCreditosVencidosPor(Guid participanteId, DateTime data);
    }
}
