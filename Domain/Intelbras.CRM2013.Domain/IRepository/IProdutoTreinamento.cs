using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IProdutoTreinamento<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid treinamentoId);
        List<T> ListarPorProduto(Guid produtoId);
        List<T> ListarTodos();
        T ObterPor(Guid produtoTreinamentoId);
        void AlterarStatus(Guid produtoTreinamentoId, int status);
        T ObterPorTreinamento(Guid TreinamentoId);
        List<T> ListarPorTreinamento(Guid TreinamentoId, Guid unidadeNegocioId);
    }
}
