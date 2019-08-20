using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IProdutoPortfolio<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid portfolioId, List<Guid> lstProduto);        
        List<T> ListarPor(List<Guid> listId);
        List<T> ListarPorProduto(Guid produtoId);
        T ObterPor(Guid produtoPortfolioId);
        void AlterarStatus(Guid produtoPortfolioId, int status);
    }
}
