using System;
using System.Collections.Generic;
using System.Data;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IOrcamentodoCanalporSegmento<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarSegmentodoOrcamentounidade(Guid orcamentounidadeId);
        List<T> ListarSegmentodoOrcamentocanal(Guid orcamentounidadeId);
        T ObterPor(Guid orcamentodocanalId, Guid canalId, int trimestre,Guid segmentoId);
        T ObterPor(Guid unidadenegocioId, int ano, int trimestre, Guid canalId, Guid segmentoId);
        List<T> ListarCanalSegmento(int ano, int trimestre);
        DataTable ListarCanalSegmentoDW(int ano, int trimestre, List<OrcamentodaUnidade> lstOrcamentodaUnidade);
    }
}

