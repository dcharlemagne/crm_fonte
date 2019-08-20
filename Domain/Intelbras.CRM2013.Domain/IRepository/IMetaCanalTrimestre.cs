using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IMetadoCanalPorTrimestre<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Enum.OrcamentodaUnidade.Trimestres trimestre);

        List<MetadoCanalporTrimestre> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano);
        List<MetadoCanalporTrimestre> ListarValoresPorUnidadeNegocioResunmida(Guid unidadeNegocioId, int ano);
    }
}