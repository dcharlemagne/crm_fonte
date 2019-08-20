using System;
using System.Collections.Generic;
using System.Data;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IOrcamentodoCanalDetalhadoporProduto<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ObterOrcCanalDetalhadoProdutos(Guid orcamentocanalprodutoId, int trimestre);
        List<T> ObterDetalhadoProdutos(Guid orcamentounidadeId);
        List<T> ObterDetalhadoProdutosManual(Guid orcamentounidadeId,Guid canalId);
        T ObterOrcamentoProdutoDetalhadoManual(Guid orcamentodocanalId,Guid unidadenegocioId, Guid canalId, int ano, int trimestre, int mes);
        List<T> ListarDetalheProdutosPorCanal(Guid orcamentounidadeId, Guid canalId,Guid produtoId);
        List<T> ListarProdutoDetalhadoCanal(int ano, int trimestre);
        DataTable ListarProdutoDetalhadoCanalDW(int ano, int trimestre);
        DataTable ListarProdutoDetalhadoCanalDW(int ano, int trimestre, List<OrcamentodaUnidade> lstOrcamentodaUnidade);
        T ObterOrcamentoProdutoDetalhado(Guid unidadenegocioId, Guid canalId, Guid produtoId, int ano, int trimestre, int mes);
    }
}

