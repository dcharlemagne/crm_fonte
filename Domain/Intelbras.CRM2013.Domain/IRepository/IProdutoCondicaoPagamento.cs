using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IProdutoCondicaoPagamento<T> : IRepository<T>, IRepositoryBase
    {
        void AlterarStatus(Guid produtoCondicaoPagamentoId, int status);
    }
}
