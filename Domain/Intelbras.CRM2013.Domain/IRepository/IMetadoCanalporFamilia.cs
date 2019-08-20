using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IMetadoCanalporFamilia<T> : IRepository<T>, IRepositoryBase
    {
        T Obterpor(Guid canalId,Guid metadocanalporsegmentoId,Guid segmentoId,Guid familiaId,int trimestre);
        T Obterpor(Guid unidadenegocioId, Guid canalId, int ano, int trimestre, Guid segmentoId, Guid familiaId);
        List<T> ListarPor(Guid metaunidadeID);
        List<T> ListarPorSegmento(Guid segmentoId);
        List<T> ListarMetaCanalFamilia(int ano, int trimestre);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres trimestre, Guid? familiaProdutoId = null);

        List<MetadoCanalporFamilia> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Guid canalId);
        DataTable ListarMetaCanalFamiliaDW(int ano, int trimestre, List<MetadaUnidade> lstMetadaUnidade);
    }
}