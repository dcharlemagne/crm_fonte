using System;
using System.Collections.Generic;
using System.Data;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IOrcamentodaUnidadeDetalhadoporProduto<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ObterOrcDetalhadoProdutos(Guid orcamentoprodutoId);
        List<T> ObterOrcDetalhadoProdutos(Guid orcamentoprodutoId, int trimestre);
        List<T> ObterOrcDetalhadoProdutos(Guid orcamentoprodutoId, int trimestre, Guid produtoid);
        List<T> ObterOrcDetalhadoProdutos(Guid produtoId, Guid orcamentounidadeId);
        List<T> ObterProdutosDetalhados(Guid orcamentounidadeId);
        List<T> ListarOrcamentoProdutoDetalhado(int ano, int trimestre);
        T ObterOrcamentoProdutoDetalhado(Guid orcamentodoprodutoId, int mes);
        DataTable ListarOrcamentoProdutoMesDW(int ano, int trimestre, List<OrcamentodaUnidade> lstOrcamentodaUnidade);
    }
}

