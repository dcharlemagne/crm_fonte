using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IProdutoPedido<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid id);
        List<T> ListarPorPedido(Guid salesorderid);
        T ObterPor(Guid id);
        T ObterPorChaveIntegracao(String chaveIntegracao);
        //bool AlterarStatus(Guid id, int status);
    }
}
