using System;
using System.Collections.Generic;
using System.Data;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IOrcamentodoCanalporSubFamilia<T> : IRepository<T>, IRepositoryBase
    {

        T ObterPorSubFamiliaTrimestreCanal(Guid subfamiliaId, Guid canalId, Guid orcunidadetrimestreId);
        List<T> ListarOrcamentoSubFamiliapor(Guid orcamentofamiliaId);
        T ObterPorSubFamiliaTrimestreCanal(Guid canalId, Guid familiaId, Guid orcamentodocanalfamiliaId,Guid segmentoId,Guid subfamiliaId,int trimestre);
        T Obter(Guid unidadenegocioId, int ano, int trimestre, Guid canalId, Guid segmentoId, Guid familiaId, Guid subfamiliaId);
        List<T> ListarCanalSubFamilia(int ano, int trimestre);
        DataTable ListarCanalSubFamiliaDW(int ano, int trimestre, List<OrcamentodaUnidade> lstOrcamentodaUnidade);
    }
}

