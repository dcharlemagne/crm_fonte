using System;
using System.Collections.Generic;
using System.Data;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IMetadaUnidadeporTrimestre<T> : IRepository<T>, IRepositoryBase
    {
        T Obter(Guid metaId, int trimestre);
        List<T> Listarpor(Guid metaunidadeId);
        T Obterpor(Guid metaunidadeId, Guid trimestreId);
        List<T> ListarMetasTrimestre(int ano, int trimestre);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano);
        List<MetadaUnidadeporTrimestre> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano);
        DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<MetadaUnidade> lstMetas);
        T Obterpor(Guid unidadenegocioId, int ano, int trimestre);
    }
}