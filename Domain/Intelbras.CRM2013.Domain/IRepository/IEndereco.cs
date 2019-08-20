using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Enum;


namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IEndereco<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(int statecode);
        T ObterPor(Guid conta, string codigoEndereco);
        T ObterPorEnderecoId(string enderecoid);
        T ObterPor(ClienteParticipanteEndereco clienteParticipanteEndereco);
        T ObterPor(string codigo, Model.Conta cliente);
        List<T> ListarPor(ClienteParticipanteDoContrato clienteParticipanteDoContrato);
        List<Model.Endereco> ObterTodosOsEnderecosPor(Domain.Model.Conta cliente);
        Model.Endereco PesquisarEnderecoPor(string cep);        
    }
}
