using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IProdutoEstabelecimento<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid estabelecimentoId);
        List<T> ListarTodos();
        List<T> ListarPorProduto(Guid? produtoId);
        List<T> ListarPorProdutoEstabelecimento(Guid produtoId,Guid itbc_estabelecimentoid);
        List<T> ListarPorEstabelecimento(Guid itbc_estabelecimentoid);
        T ObterPor(Guid produtoEstabelecimentoId);
        T ObterPorProduto(Guid? produtoId);
        bool AlterarStatus(Guid id, int status);
    }
}