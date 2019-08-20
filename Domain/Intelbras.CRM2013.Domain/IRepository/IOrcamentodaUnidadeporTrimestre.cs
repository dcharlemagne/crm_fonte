using System;
using System.Collections.Generic;
using System.Data;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IOrcamentodaUnidadeporTrimestre<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ObterOrcamentoTrimestre(Guid orcamentounidadeId);
        T ObterOrcamentoTrimestre(Guid orcamentounidadeId,int trimestre);
        T ObterOrcamentoTrimestre(Guid orcamentounidadeId, int ano, int trimestre);
        T ObterOrcamentoTrimestre(Guid orcamentounidadeId, Guid trimestreId);
        List<T> ListarOrcamentoTrimestre(int ano);
        List<T> ListarOrcamentoTrimestre(int ano, int trimestre);
        DataTable ObterOrcamentoTrimestreDW(int ano);
        DataTable ObterOrcamentoTrimestreDW(int ano, int trimestre, List<OrcamentodaUnidade> lstOrcamentodaUnidade);
    }
}

