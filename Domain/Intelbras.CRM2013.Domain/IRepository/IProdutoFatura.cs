using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IProdutoFatura<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid accountid);
        List<T> listarContagemVendaPrice(Guid customerid, List<Guid?> lstProdutos);
        T ObterPor(Guid accountid);
        T ObterPor(Guid produtoID,Guid faturaID);
        T ObterPorChaveIntegracao(String chaveIntegracao);
        bool AlterarProprietario(Guid proprietario, string TipoProprietario, Guid prodFaturaId);
        T ObterObtemPorNotaFiscal(Guid prodExistId, Guid nfeId);
        List<T> ListarProdutosDaFaturaPor(Guid invoiceid);
    }
}
