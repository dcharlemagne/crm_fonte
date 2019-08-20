using System;
using System.Collections.Generic;
using System.Data;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IOrcamentodaUnidadeporFamilia<T> : IRepository<T>, IRepositoryBase
    {
        T ObterOrcamentoFamilia(Guid familiaId,Guid segmentoId);
        T ObterOrcamentoFamilia(Guid unidadenegocioId, int ano, int trimestre, Guid segmentoId, Guid familiaId);
        List<T> ObterOrcamentoFamiliaporOrcUnidade(Guid orcamentounidadeId);
        List<T> ObterOrcamentoFamiliaporSegmento(Guid segmentoId);
        List<T> ListarOrcamentoFamilia(int ano, int trimestre);
        DataTable ListarOrcamentoFamiliaDW(int ano, int trimestre, List<OrcamentodaUnidade> lstOrcamentodaUnidade);
    }
}

