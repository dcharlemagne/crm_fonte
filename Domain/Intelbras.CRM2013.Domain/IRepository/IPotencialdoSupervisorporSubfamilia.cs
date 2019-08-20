using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPotencialdoSupervisorporSubfamilia<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarSubFamilia(Guid metaunidadeId);
        T Obterpor(Guid supervisorId, Guid familiaId, Guid potencialsupervisorfamiliaId, Guid segmentoId, Guid subfamiliaId, int trimestre);
        T Obter(Guid subfamiliaId, Guid supervisorId, Guid trimestreId);
        List<PotencialdoSupervisorporSubfamilia> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres? trimestre = null);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres? trimestre = null);
        List<T> ListarSubFamiliaPor(Guid potencialdokafamiliaId);
        T Obter(Guid unidadenegocioId, Guid supervisorId, int ano, int trimestre, Guid segmentoId, Guid familiaId, Guid subfamiliaId);
        DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<MetadaUnidade> lstMetas);
    }
}