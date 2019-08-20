using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IMetadaUnidadeporProdutoMes<T> : IRepository<T>, IRepositoryBase
    {
        T Obter(Guid produtoId, Guid metaunidadesubfamiliaId);
        List<T> ListarPor(Guid produtoId);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes? mes = null);
        List<T> ListarPorCanal(Guid canalId);

        List<MetaDetalhadadaUnidadeporProduto> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes mes);
    }
}