using System;
using System.Collections.Generic;
using System.Data;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IOrcamentodaUnidadeporProduto<T> : IRepository<T>, IRepositoryBase
    {
        T ObterOrcamentoporProduto(Guid produtoId, Guid orcamentosubfamiliaId);
        T ObterOrcUnidadeProduto(Guid produtoId, Guid trimestreId);
        List<T> ListarOrcUnidadeProduto(Guid subfamiliaId);
        List<T> ListarProdutosOrcamento(Guid orcamentounidadeId);
        List<T> ListarProdutosOrcamento(Guid orcamentounidadeId, List<Guid> lstIdProdutos);
        List<T> ListarOrcamentoProduto(int ano, int trimestre);
        T ObterOrcamentoporProduto(Guid produtoId, Guid unidadenegocioId, int ano, int trimestre, string codsegmento, string codfamilia, string codsubfamilia);
        DataTable ListarOrcamentoProdutoDW(int ano, int trimestre, List<OrcamentodaUnidade> lstOrcamentodaUnidade);
    }
}

