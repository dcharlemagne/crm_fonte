using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IClienteParticipanteDoEndereco<T> : IRepository<T>, IRepositoryBase
    {
        List<T> PesquisarPor(ClienteParticipante clienteParticipante);
        List<T> ListarPor(ClienteParticipanteDoContrato clienteParticipanteDoContrato);
        List<T> ListarPor(Model.Conta cliente, Contrato contrato);
        List<T> ListarPor(Model.Conta cliente, Contrato contrato, string endereco);
        void DeleteAll(Contrato contrato);
    }
}
