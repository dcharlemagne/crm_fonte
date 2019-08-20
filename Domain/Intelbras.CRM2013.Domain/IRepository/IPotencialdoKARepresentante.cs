using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPotencialdoKARepresentante<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano);
        List<PotencialdoKARepresentante> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano);
        T Obter(Guid metadaunidadeportrimestreId, Guid KARepresentanteId, int trimestre);
        T Obter(Guid unidadenegocioId, Guid KARepresentanteId, int ano, int trimestre, params string[] columns);
        T Obter(Guid unidadenegocioId, Guid KARepresentanteId, int ano, params string[] columns);
        DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<UnidadeNegocio> lstMetas);
        DataTable ListarMetaTrimestreDW(int ano, List<UnidadeNegocio> lstMetas);
        DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<MetadaUnidade> lstMetas);
        DataTable ListarMetaTrimestreDW(int ano, List<MetadaUnidade> lstMetas);
        T ObterPor(Guid KARepresentanteId, Guid UNId, int ano);
        T ObterPorSegmento(Guid segmnetoId);
    }
}
