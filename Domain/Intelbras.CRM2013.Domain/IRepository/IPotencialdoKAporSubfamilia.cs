using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPotencialdoKAporSubfamilia<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListsarSubfamilia(Guid metaunidadeId);
        T Obterpor(Guid canalId, Guid familiaId, Guid metadocanalporfamiliaId, Guid segmentoId, Guid subfamiliaId, int trimestre);
        T Obter(Guid subfamiliaId, Guid karepresentanteId, Guid trimestreId);
        List<T> ObterSubFamiliaPor(Guid potencialdokafamiliaId);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano);
        List<PotencialdoKAporSubfamilia> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Enum.OrcamentodaUnidade.Trimestres trimestre);
        T Obter(Guid unidadenegocioId, Guid KARepresentanteId, int ano, int trimestre, Guid segmentoId, Guid familiaId, Guid subfamiliaId);
        DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<MetadaUnidade> lstMetas);
    }
}