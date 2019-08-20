using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using System.Data;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPotencialdoSupervisorporSegmento<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid metadaunidadeId);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres? trimestre = null);
        List<PotencialdoSupervisorporSegmento> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Enum.OrcamentodaUnidade.Trimestres? trimestre = null);
        T Obter(Guid potencialdosupervisorId, Guid supervisorId, int trimestre, Guid segmentoId);
        T Obter(Guid unidadenegocioId, Guid supervisorId, int ano, int trimestre, Guid segmentoId);
        List<T> Listar(Guid potencialdosupervisorId);
        DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<MetadaUnidade> lstMetas);
    }
}