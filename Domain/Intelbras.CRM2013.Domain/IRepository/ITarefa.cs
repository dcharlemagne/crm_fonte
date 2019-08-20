using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ITarefa<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid activityid);
        T ObterPor(Guid activityid);
        bool AlterarStatus(Guid activityid, int status, int? statusCode);
        List<T> ListarPor(Guid referenteA, Guid tipoAtividade, DateTime? dtInicial, DateTime? dtFim, int? situacao);
        T ObterPorReferenteA(Guid referenteA);
        List<T> ListarPorReferenteA(Guid referenteA);
        List<T> ListarPorReferenteAAtivo(Guid referenteA);
        List<T> TarefasPorSolicitacao(Guid solicId);
    }
}
