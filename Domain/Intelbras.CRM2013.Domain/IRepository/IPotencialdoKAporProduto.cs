using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPotencialdoKAporProduto<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarProdutos(Guid potencialkasubfamiliaId);
        List<T> ObterProdutos(Guid metaunidadeId, List<Guid> lstIdProdutos);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano);
        List<PotencialdoKAporProduto> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes mes);
        List<PotencialdoKAporProduto> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Guid representanteId);
        T Obter(Guid CanalId, Guid produtoId, Guid subfamiliaId, Guid potencialdokaporsubfamiliaId, int Trimestre);
        T Obter(Guid unidadenegocioId, Guid KARepresentanteId, int ano, int trimestre, Guid produtoId);
        T ObterPor(Guid KARepresentanteId, Guid produtoId, int trimestreID);
        DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<MetadaUnidade> lstMetas);
    }
}