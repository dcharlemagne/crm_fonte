using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IMetadoCanalporSegmento<T> : IRepository<T>, IRepositoryBase
    {
        T Obter(Guid unidadenegocioId, Guid canalId, int ano, int trimestre, Guid segmentoId, params string[] columns);
        List<T> ListarPor(Guid metaunidadeId);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres trimestre);
        List<MetadoCanalporSegmento> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres trimeste);

        DataTable ListarMetaCanalSegmentoDW(int ano, int trimestre, List<MetadaUnidade> lstMetadaUnidade);
    }
}