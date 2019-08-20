using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IMetadoCanalporSubFamilia<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarSubFamilia(Guid metaunidadeId);
        T Obterpor(Guid canalId, Guid familiaId, Guid metadocanalporfamiliaId, Guid segmentoId, Guid subfamiliaId, int trimestre);
        T ObterPor(Guid canalId, Guid subfamiliaId, Guid metadotrimestreId);
        T Obterpor(Guid unidadenegocioId, Guid canalId, int ano, int trimestre, Guid segmentoId, Guid familiaId, Guid subfamiliaId);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres trimestre, Guid familiaProdutoId);
        List<T> ListarPorUnidadeNegocioSubfamilia(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres trimestre, Guid subfamiliaProdutoId);
        List<T> ListarPor(Guid metacanalfamiliaId);
        List<T> ListarMetaCanalSubFamilia(int ano, int trimestre);

        List<MetadoCanalporSubFamilia> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Guid canalId);

        DataTable ListarMetaCanalSubFamiliaDW(int ano, int trimestre, List<MetadaUnidade> lstMetadaUnidade);
    }
}