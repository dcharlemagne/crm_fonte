using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPotencialdoKAporSegmento<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid metadokarepresentanteId);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocio, int ano);
        T Obter(Guid potencialdokarepresentanteId, Guid karepresentante, int trimestre, Guid segmentoId);
        List<T> ListarSegmentos(MetadaUnidade metaunidade, Guid representantId);
        List<PotencialdoKAporSegmento> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano);
        T Obter(Guid unidadenegocioId, Guid KARepresentanteId, int ano, int trimestre, Guid segmentoId);
        DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<MetadaUnidade> lstMetas);
        T ObterPor(Guid segmentoId, Guid potencialkasegmentoId, Guid trimestreId);
        List<T> ListarPorTrimestreId(Guid TrimestreId);
        List<T> __Obter(Guid unId, Guid representanteId, int trimestre, Guid trimestreId, Guid segId);
    }
}