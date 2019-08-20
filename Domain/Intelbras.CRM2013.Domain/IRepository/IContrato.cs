using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.Servicos.GestaoSLA;
using Intelbras.CRM2013.Domain.ValueObjects;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IContrato<T> : IRepository<T>, IRepositoryBase
    {
        void SalvarEnderecosParticipantes(Guid clienteParticipanteId, Contrato contrato, Model.Conta cliente, Domain.Model.Endereco endereco);
        bool ExisteEndereco(Contrato contrato, Guid clienteId, Guid enderecoId);
        CalendarioDeTrabalho ObterCalendario(Guid contratoId);
        List<ClienteParticipante> ObterEnderecoClientesParticipantesPor(Contrato contrato, Model.Conta cliente);
        Guid ObterEnderecoIDPrimeiroClienteParticipante(Contrato contrato, Model.Conta cliente);
        List<ClienteParticipante> ObterClientesParticipantesPor(Contrato contrato, Model.Conta cliente);
        List<ClienteParticipante> ObterClientesParticipantesPor(Contrato contrato);
        ClienteParticipante PesquisarParticipantePor(Contrato contrato, Model.Conta cliente);
        ClienteParticipante PesquisarParticipantePor(Contrato contrato, Model.Conta cliente, Domain.Model.Endereco endereco);
        ClienteParticipante PesquisarEnderecoParticipantePor(Guid enderecoparticipanteId);
        void AlterarStatusDoContrato(Guid contratoId, int statusContrato);
        void ExcluirEnderecosSemLocalidadePor(ClienteParticipante clienteSemLocalidade);
        Guid SalvarClienteParticipante(ClienteParticipante clienteParticipante);
        List<T> ListarPorClientesParticipantes(Model.Conta cliente, List<StatusDoContrato> status);
        List<T> ListarPorPerfilUsuarioServico(Intelbras.CRM2013.Domain.Model.Contato contato);
        List<ClienteDoContrato> ListarClientesParticipantes(ClienteDoContrato clienteDoContrato, int pagina, int quantidade, ref bool existemMaisRegistros, ref string cookie);
        T ObterPor(Ocorrencia ocorrencia);
        List<T> ObterContratosProximoVencimento(DateTime dataTermino);
    }
}