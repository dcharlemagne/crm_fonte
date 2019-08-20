using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IProdutoKit<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPorProdutoPai(Guid produtoPaiId);
        List<T> ListarPorProdutoPai(string codigoProdutoPai);
        bool AlterarStatus(Guid id, int statusCode);
    }
}