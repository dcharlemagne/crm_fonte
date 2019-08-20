using System;
using System.Collections.Generic;
using System.Data;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IOrcamentodaUnidade<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ObterOrcamentoParaGerarPlanilha(int status);
        List<T> ObterOrcamentoParaGerarPlanilhaManual(int status);
        List<T> ListarOrcamentos(int ano);
        List<T> ListarOrcamentos(Guid unidadenegocioId, int ano);
        void ListarOrcamentosRealizados();
        DataTable ObterCapaDW(int ano);
        DataTable ObterCapaDW(int ano, List<OrcamentodaUnidade> lstOrcamento);
    }
}

