using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPotencialdoKAporFamilia<T> : IRepository<T>, IRepositoryBase
    {
        T Obter(Guid segmentoId, Guid familiaId, Guid potencialkasegmentoId, Guid karepresentanteId, int trimestre);
        List<T> ObterFamiliaporMeta(Guid metaunidadeId);
        List<T> ObterFamiliaporSegmento(Guid potencialdokaporsegmentoId);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano);
        List<PotencialdoKAporFamilia> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano);
        T ObterPor(Guid unidadenegocioId, Guid KARepresentanteId, int ano, int trimestre, Guid segmentoId, Guid familiaId);
        DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<MetadaUnidade> lstMetas);
        T ObterPor(Guid segmentoId, Guid familiaId, Guid representanteId, int trimestre);
    }
}