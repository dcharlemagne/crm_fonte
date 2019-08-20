using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IMetadaUnidadeporSegmento<T> : IRepository<T>, IRepositoryBase
    {
        T Obter(Guid segmentoId, Guid metaunidadetrimestreId);
        T ObterMetasSegmento(Guid unidadenegocioId, Guid segmentoId, int ano, int trimestre, params string[] columns);
        List<T> Obter(Guid metaunidadeId);
        List<T> Obterpor(Guid metaunidadetrimestreId);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres? trimestre = null);


        List<MetadaUnidadeporSegmento> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres? trimestre = null);

        DataTable ListarMetasSegmentoDW(int ano, int trimestre);
        DataTable ListarMetasSegmentoDW(int ano, int trimestre, List<MetadaUnidade> lstOrcamentodaUnidade);
    }
}