using System;
using System.Collections.Generic;
using System.Data;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IVeiculo<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPorClienteParticipanteContrato(Guid clienteParticipanteId);
        List<T> ListarPorClientesParticipantesContrato(List<ClienteParticipante> lstClienteParticipante, string placa);
        T ObterPorPlacaVeiculo(string numeroplaca);
    }
}
