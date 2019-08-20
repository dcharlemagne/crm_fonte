using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPotencialdoSupervisor<T> : IRepository<T>, IRepositoryBase
    {
        List<T> Listar(Guid metaunidadeId);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano);
        List<PotencialdoSupervisor> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano);
        T Obter(Guid metadaunidadeportrimestreId, Guid supervisorId, int trimestre);
        T ObterPor(Guid unidadenegocioId, Guid supervisorId, int ano, int trimestre, params string[] columns);
        DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<MetadaUnidade> lstMetas);
    }
}