using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPortfolio<T> : IRepository<T>, IRepositoryBase
    {
        //List<T> ListarPor(Guid? unidadeNegocioId, int? tipo);
        // List<T> ListarPor(List<Guid> unidadesNegocioIds, List<int> tipo);
        //List<T> ListarPor(Guid? unidadeNegocioId, int? tipo, Guid? classificacao, Guid? categoria, Guid portfolioId);

        List<T> ListarPor(Guid? unidadeNegocioId, int? tipo, Guid? classificacao);
        List<T> ListarPor(Guid? unidadeNegocioId, int? tipo, Guid? classificacaoId, Guid portfolioId);
        List<T> ListarPor(List<Guid> unidadesNegocioIds, int tipo, Guid? classificacaoId);
        List<T> ListarPor(List<Guid> unidadesNegocioIds, int tipo, List<Guid> classificacaoIds);
        List<T> ListarPor(List<Guid> unidadesNegocioIds, List<int> tipo, Guid? classificacaoId);
        List<T> ListarPor(List<Guid> unidadesNegocioIds, List<int> tipo, List<Guid> classificacaoId);
        List<T> ListarPorCrosseling(List<Guid> unidadesNegocioIds, int tipo, Guid? classificacaoId);
        List<T> ListarPorProduto(Guid produtoId, bool apenasAtivos = true, params string[] columns);
        T ObterPor(Guid portfolioId);
    }
}
