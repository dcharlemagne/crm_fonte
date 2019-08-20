using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IClienteParticipanteDoContrato<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Contrato contrato, Domain.Model.Conta cliente);
        List<T> ListarPor(Contrato contrato);
        void DeleteAll(Contrato contrato);
    }
}
