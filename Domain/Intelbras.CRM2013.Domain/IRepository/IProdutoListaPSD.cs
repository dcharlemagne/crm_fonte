using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IProdutoListaPSD<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid listaPrecoPSDId, Guid? productid);
        T ObterPor(Guid produtoListaId);
        T ObterPor( Guid Produto, Guid EstadoProduto, Guid UnidadeNegocioProduto);
    }
}
