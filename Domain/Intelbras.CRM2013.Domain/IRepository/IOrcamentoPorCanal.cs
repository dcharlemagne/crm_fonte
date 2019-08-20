using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IOrcamentoPorCanal<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(Guid orcamentoporcanalId);
        List<T> ListarPorOrcamentoUnidade(Guid orcamentounidadeId);
        T ObterPor(Guid orcamentounidadetrimestreId,Guid canalId,int trimestre);
        T ObterPor(Guid unidadenegocioId, int ano, int trimestre, Guid canalId);
        List<T> ListarPorOrcamentoUnidade(Guid orcamentounidadeId, List<Guid> lstIdCanal);
        List<T> ListarCanal(int ano, int trimestre);
        DataTable ListarCanalDW(int ano, int trimestre, List<OrcamentodaUnidade> lstOrcamentodaUnidade);
    }
}
