using System;
using System.Collections.Generic;
using System.Data;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IOrcamentodoCanalporFamilia<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarOrcamentoFamiliaporOrcUnidade(Guid orcamentounidadeId);
        List<T> ListarOrcamentoFamiliapor(Guid orcamentosegmentocanalId);
        T ObterOrcamentoCanalFamilia(Guid canalId, Guid orcamentocanalsegmentoId, Guid segmentoId, Guid familiaId, int trimestre);
        T Obter(Guid unidadenegocioId, int ano, int trimestre, Guid canalId, Guid segmentoId, Guid familiaId);
        List<T> ListarCanalFamilia(int ano, int trimestre);
        DataTable ListarCanalFamiliaDW(int ano, int trimestre, List<OrcamentodaUnidade> lstOrcamentodaUnidade);
    }
}

