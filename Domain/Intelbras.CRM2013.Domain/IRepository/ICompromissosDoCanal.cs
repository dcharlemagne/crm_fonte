using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ICompromissosDoCanal<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(Guid CompromissosDoCanalId);
        T ObterPor(Guid compromissoId, Guid canalId);
        T ObterPorNome(string compromisso, Guid canalId, Guid unidadeId);
        T ObterPor(Guid compromissoId, Guid unidNeg, Guid canalId);
        T ObterPor(Guid unidadeNegocioId, Guid contaId, int codigoCompromisso);
        List<T> ListarPor(Guid CompromissosDoCanalId);
        List<T> ListarPorCanal(Guid accountId);
        List<T> ListarPor(Guid canalId, Guid unidadeNegocioId);
        List<T> ListarPorCanalCompromisso(Guid canalId, Guid compromissoId);
        List<T> ListarPor(List<Guid> CompromissosProg, Guid Canal, Guid UnidadeNegocios);
        List<T> ListarPorContaUnidade(Guid canalId, Guid unidadeNegocioId);
        List<T> ListarPorPlanilha();
        List<T> ListarLote();
        List<T> ListarPorCod33EPorMatriz(Guid UnidadeId, Guid canalId);
        Boolean AtualizarStatus(Guid compromissoID, int stateCode, int statusCode);
        List<T> ListarAtivosVencidosCumpridos(Intelbras.CRM2013.Domain.Enum.CompromissoPrograma.TipoMonitoramento tipoMonitoramento, Domain.Model.StatusCompromissos cumprido, Domain.Model.StatusCompromissos cumpridoForaPrazo);
        List<T> ListarAtivosCumpridos(Intelbras.CRM2013.Domain.Enum.CompromissoPrograma.TipoMonitoramento tipoMonitoramento, Domain.Model.StatusCompromissos cumprido, Domain.Model.StatusCompromissos cumpridoForaPrazo);
        List<T> ListarVencidosManualPorTarefasESolicitacoes(int[] tipoMonitoramento, Guid statusCompromissoId);
        List<T> ListarPorSoliBenDeShowRoom(Guid solId, Guid compromissoId);
    }
}
