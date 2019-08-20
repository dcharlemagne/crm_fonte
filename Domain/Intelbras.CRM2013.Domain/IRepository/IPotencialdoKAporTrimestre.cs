using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPotencialdoKAporTrimestre<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid metadokarepresentanteId, int trimestre);
        T Obter(Guid potencialdokarepresentanteId, Guid karepresentante, int trimestre, Guid segmentoId);
        List<T> Obter(Guid potencialKA);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano);
        List<T> ListarPorTrimestre(int ano, int trimestre);
        List<PotencialdoKAporTrimestre> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano);
        T Obter(Guid unidadenegocioId, Guid KARepresentanteId, int ano, int trimestre);
        DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<MetadaUnidade> lstMetas);
        DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<UnidadeNegocio> lstMetas);
        T ObterPor(Guid potencialKAId, Guid karepresentante, int trimestre);
    }
}