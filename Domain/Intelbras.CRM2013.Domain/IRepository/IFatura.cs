using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IFatura<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid accountid);
        T ObterPor(Guid accountid);
        T ObterPorPedidoEMS(String pedidoEMS);
        T ObterPorNumeroNF(String numeroNF);
        T ObterPorChaveIntegracao(String chaveIntegracao, params string[] columns);
        bool AlterarStatus(Guid invoice, int statecode, int statuscode);
        bool AlterarProprietario(Guid proprietario, string TipoProprietario, Guid invoice);
        //CRM4
        List<T> ListarNotasFiscaisPor(Model.Conta cliente);
        T ObterPor(Contrato contrato);
        Fatura PesquisarNotaFiscalFaturaDaOcorrenciaPor(Contrato contrato, Domain.Model.Conta cliente);
        T ObterFaturaPor(Contrato contrato, Model.Conta cliente);
        T ObterRemessaPor(Contrato contrato, Model.Conta cliente);
        T ObterRemessaPor(Contrato contrato);
        List<ProdutoFatura> ListarItensDaNotaFiscalPor(Guid id);
        //CRM4
    }
}
