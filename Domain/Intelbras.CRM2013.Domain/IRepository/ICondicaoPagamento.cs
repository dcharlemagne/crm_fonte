using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ICondicaoPagamento<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(int? stateCode);
        T ObterPor(Guid condicaoPagamentoId);
        T ObterPor(int codigo);
        T ObterPor(Pedido pedido);
        void AlterarStatus(Guid condicaoPagamentoId, int state, int status, Guid usuarioId);
    }
}
