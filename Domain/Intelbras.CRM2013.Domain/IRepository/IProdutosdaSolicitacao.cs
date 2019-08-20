using System;
using System.Collections.Generic;
using System.Data;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IProdutosdaSolicitacao<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(Guid produtoSolicitacaoId);
        T ObterAtivoInativoPor(Guid produtoSolicitacaoId);
        Boolean AlterarStatus(Guid id, int stateCode, int statusCode);
        List<T> ListarPorSolicitacao(Guid solicitacaoId);
        List<T> ListarPorSolicitacaoAtivos(Guid solicitacaoId);

    }
}
