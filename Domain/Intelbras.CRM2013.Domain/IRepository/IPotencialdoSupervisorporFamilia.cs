using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPotencialdoSupervisorporFamilia<T> : IRepository<T>, IRepositoryBase
    {
        T Obter(Guid segmentoId, Guid familiaId, Guid potencialdosupervisorporsegmentoId, Guid supervisorId, int trimestre);
        List<T> ListarFamiliaporMeta(Guid metaunidadeId);
        List<T> ListarFamiliaporSegmento(Guid potencialdokaporsegmentoId);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres? trimestre = null);
        List<PotencialdoSupervisorporFamilia> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres? trimestre = null);
        T Obter(Guid unidadenegocioId, Guid supervisorId, int ano, int trimestre, Guid segmentoId, Guid familiaId);
        DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<MetadaUnidade> lstMetas);
    }
}