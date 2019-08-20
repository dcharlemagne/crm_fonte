using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IMetadaUnidadeporFamilia<T> : IRepository<T>, IRepositoryBase
    {
        T Obter(Guid familiaId, Guid metaunidadeporsegmentoId);
        List<T> ObterFamiliaporMeta(Guid metaunidadeId);
        List<T> ObterFamiliaporSegmento(Guid segmentoId);
        List<T> ListarMetaFamilia(int ano, int trimestre);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres? trimestre = null);
        T ObterMetaFamilia(Guid unidadenegocioId, Guid segmentoId, Guid familiaId, int ano, int trimestre);

        List<MetadaUnidadeporFamilia> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres? trimestre = null);

        DataTable ListarMetaFamiliaDW(int ano, int trimestre);
        DataTable ListarMetaFamiliaDW(int ano, int trimestre, List<MetadaUnidade> lstOrcamentodaUnidade);
    }
}