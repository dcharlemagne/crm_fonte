using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IOrcamentodaUnidadeporSegmento<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarSegmentodoOrcamento(Guid orcamentoId);
        T ObterOrcamentoSegmento(Guid segmentoId, Guid trimestreId);
        T ObterOrcamentoSegmento(Guid unidadenegocioId, int ano, int trimestre, Guid segmentoId, params string[] colunas);
        List<T> ListarSegmentodoOrcamentoproTrimestre(Guid trimestreId);
        List<T> ListarOrcamentoSegmento(int ano, int trimestre);
        DataTable ListarOrcamentoSegmentoDW(int ano, int trimestre, List<OrcamentodaUnidade> lstOrcamentodaUnidade);
    }
}

