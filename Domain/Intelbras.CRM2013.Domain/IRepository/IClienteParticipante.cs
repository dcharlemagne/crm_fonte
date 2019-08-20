using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IClienteParticipante<T> : IRepository<T>, IRepositoryBase
    {
        ClienteParticipanteDoContrato InstanciarClienteParticipanteDoContrato(ClienteParticipante cli);
        ClienteParticipanteEndereco InstanciarClienteParticipanteEndereco(ClienteParticipante cli);
        ClienteParticipante InstanciarClienteParticipante(ClienteParticipanteDoContrato cli);
        ClienteParticipante InstanciarClienteParticipante(ClienteParticipanteEndereco cli);
        ClienteParticipante ObterPorEnderecoId(Guid clienteParticipanteEnderecoId);
        ClienteParticipante ObterPor(Guid clienteId, Guid contratoId);
        List<ClienteParticipante> ListarPor(Guid clienteid);
        List<ClienteParticipante> ListarPor(Model.Conta cliente);
        List<ClienteParticipante> ListarPor(Guid clienteid, Guid enderecoid);
        ClienteParticipante ObterPor(Guid clienteParticipanteContratoId);
        List<ClienteParticipante> ListarPor(Contrato contrato);
    }
}
