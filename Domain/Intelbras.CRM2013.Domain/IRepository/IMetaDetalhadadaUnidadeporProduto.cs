using System;
using System.Collections.Generic;
using System.Data;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IMetaDetalhadadaUnidadeporProduto<T> : IRepository<T>, IRepositoryBase
    {
        List<T> Listar(Guid metaunidadeprodutoId);
        List<T> ObterDetalhadoProdutos(Guid produtoId, Guid metaunidadeId);
        List<T> Obterpor(Guid metaprodutoId, int trimestre);
        T ObterProdutoDetalhado(Guid unidadenegocioId, Guid produtoId, int ano, int trimestre, int mes);
        DataTable ListarProdutoDetalhadoDW(int ano, int trimestre);
        DataTable ListarProdutoDetalhadoDW(int ano, int trimestre, List<MetadaUnidade> lstOrcamentodaUnidade);
    }
}

