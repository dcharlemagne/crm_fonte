using System;
using System.Collections.Generic;
using System.Data;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IMetadaUnidadeporSubfamilia<T> : IRepository<T>, IRepositoryBase
    {
        T Obter(Guid subfamiliaId, Guid metadaunidadeporfamiliaId);
        T ObterporTrimestre(Guid subfamiliaId, Guid metaunidadeId);
        List<T> ListarPor(Guid metadaunidadeId);
        List<T> ObterSubFamiliaPor(Guid metafamiliaId);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Enum.OrcamentodaUnidade.Trimestres? trimestre = null);

        List<MetadaUnidadeporSubfamilia> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres? trimestre = null);

        T ObterMetaSubFamilia(Guid unidadenegocioId, Guid segmentoId, Guid familiaId, Guid subfamiliaId, int ano, int trimestre);
        DataTable ListarMetaSubFamiliaDW(int ano, int trimestre);
        DataTable ListarMetaSubFamiliaDW(int ano, int trimestre, List<MetadaUnidade> lstOrcamentodaUnidade);
    }
}