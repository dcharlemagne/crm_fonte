using System;
using System.Collections.Generic;
using System.Data;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IOrcamentodoCanalporProduto<T> : IRepository<T>, IRepositoryBase
    {
        T ObterOrcamentoCanalporProduto(Guid produtoId, Guid orccanalsubfamiliaId);
        T ObterOrcCanalporProduto(Guid canalId, Guid produtoId, Guid trimestreId);
        T ObterOrcCanalProduto(Guid canalId, Guid produtoId, Guid orccanalsubfamiliaId,int trimestre);
        List<T> ListarProdutoCanalOrcamento(Guid orcamentounidadeId);
        List<T> ListarProdutoCanalOrcamento(Guid orcamentounidadeId, List<Guid> lstIdProdutos);
        List<T> ListarProdutoCanalPorOrcamentoUnidade(Guid orcamentounidadeId, List<Guid> lstIdCanal);
        T ObterOrcCanalProduto(Guid unidadenegocioId, int ano, int trimestre, Guid canalId, Guid produtoId);
        List<T> ListarCanalProduto(int ano, int trimestre);
        DataTable ListarCanalProdutoDW(int ano, int trimestre);
        DataTable ListarCanalProdutoDW(int ano, int trimestre, List<OrcamentodaUnidade> lstOrcamentodaUnidade);
    }
}

