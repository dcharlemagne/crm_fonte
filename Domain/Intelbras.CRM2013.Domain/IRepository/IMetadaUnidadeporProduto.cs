using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IMetadaUnidadeporProduto<T> : IRepository<T>, IRepositoryBase
    {
        T Obter(Guid produtoId, Guid metaunidadesubfamiliaId);
        T ObterporTrimestre(Guid produtoId, Guid metaunidadetrimestreId);
        List<T> ListarPor(Guid metaunidadesubfamiliaId);
        List<T> ObterProdutos(Guid metaunidadeId, List<Guid> lstIdProdutos);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres? trimestre = null);
        T ObterMetaProduto(Guid unidadenegocioId, Guid produtoId, int ano, int trimestre);


        List<MetadaUnidadeporProduto> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres trimestre);

        DataTable ListarMetaProdutoDW(int ano, int trimestre);
        DataTable ListarMetaProdutoDW(int ano, int trimestre, List<MetadaUnidade> lstOrcamentodaUnidade);
    }
}