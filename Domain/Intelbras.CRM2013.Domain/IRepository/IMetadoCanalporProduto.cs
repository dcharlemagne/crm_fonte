using System;
using System.Collections.Generic;
using System.Data;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IMetadoCanalporProduto<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarProdutosCanal(Guid metacanalsubfamiliaId);
        T Obter(Guid canalId, Guid produtoId, Guid subfamiliaId, Guid metaporsubfamiliaId, int ano, int trimestre);
        T Obterpor(Guid canalId, Guid produtoId, Guid trimesteId);
        T Obterpor(Guid unidadenegocioId, Guid canalId, int ano, int trimestre, Guid subfamiliaId, Guid produtoId);
        List<T> ListarProdutos(Guid metaunidadeId, List<Guid> lstIdProdutos);
        List<T> ListarCanalProdutos(Guid metaunidadeId, List<Guid> lstIdCanal);
        List<T> ListarMetaCanalProduto(int ano, int trimestre);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres trimestre, Guid subfamiliaProdutoId);
        List<T> ListarPorUnidadeNegocioProduto(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres trimestre, Guid produtoId);

        List<MetadoCanalporProduto> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Guid canalId);

        DataTable ListarMetaCanalProdutoDW(int ano, int trimestre, List<MetadaUnidade> lstMetadaUnidade);
    }
}

