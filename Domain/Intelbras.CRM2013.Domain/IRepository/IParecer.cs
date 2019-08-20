using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IParecer<T>:IRepository<T>,IRepositoryBase
    {
        List<T> ListarPor(Guid TarefaId);
        T ObterPor(Guid itbc_parecerid);
        T ObterPor(String pedidoEMS);
        void AtribuirEquipeParaPedido(Guid Equipe, Guid Pedido);
        Boolean AlterarStatus(Guid id, Domain.Enum.Pedido.StateCode stateCode, Domain.Enum.Pedido.RazaoStatus statusCode);
        Boolean AlterarStatus(Guid guidId, int status);
        
    }
}