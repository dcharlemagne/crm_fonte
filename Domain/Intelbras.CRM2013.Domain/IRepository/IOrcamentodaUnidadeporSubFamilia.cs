using System;
using System.Collections.Generic;
using System.Data;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IOrcamentodaUnidadeporSubFamilia<T> : IRepository<T>, IRepositoryBase
    {
        T ObterOrcamentoSubFamilia(Guid subfamiliaId, Guid orcamentofamiliaId);
        T ObterOrcSubFamiliaporProdTrimestre(Guid trimestreId, Guid produtoId);
        T ObterPorSubFamiliaTrimestreUnidade(Guid subfamiliaId, Guid orcunidadetrimestreId);
        T ObterPorSubFamiliaTrimestreUnidade(Guid unidadenegocioId, int ano, int trimestre, Guid segmentoId, Guid familiaId, Guid subfamiliaId);
        List<T> ListarSubFamiliaPor(Guid familiaId);
        List<T> ListarOrcamentoSubFamilia(int ano, int trimestre);
        DataTable ListarOrcamentoSubFamiliaDW(int ano, int trimestre, List<OrcamentodaUnidade> lstOrcamentodaUnidade);

    }
}

