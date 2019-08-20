using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IMetaDetalhadadoCanalporProduto<T> : IRepository<T>, IRepositoryBase
    {
        T Obter(int ano, int trimestre, int mes, Guid canalId, Guid metadocanalproprodutoId, Guid produtoId);
        T ListarPorManual(Guid canalId,int mes);
        T ObterManual(int trimestre, int ano, Guid canalId, Guid metadocanalId, int mes, Guid unidadenegocioId);
        List<T> ListarPor(Guid canalId, Guid produtoId, Guid metadotrimestreId);
        List<T> ListarPor(Guid canalId, Guid unidadeNegocioId, int ano);
        List<T> ListarDetalheProdutosPorMeta(Guid metaId, Guid canalId, Guid produtoId);
        List<T> ListarMetaCanalDetalhadoProduto(int ano, int trimestre);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Enum.OrcamentodaUnidade.Trimestres trimestre, Guid? produtoId);
        DataTable ListarMetaCanalDetalhadoProdutoDW(int ano, int trimestre, List<MetadaUnidade> lstMetadaUnidade);
        DataTable ListarMetaCanalManualDetalhadoProdutoDW(int ano, int trimestre, List<MetadaUnidade> lstMetadaUnidade);
        DataTable ListarDadosDWPor(int ano, int trimestre, string unidadeNegocioChaveIntegracao);
    }
}