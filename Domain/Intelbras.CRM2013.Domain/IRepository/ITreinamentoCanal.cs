using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ITreinamentoCanal<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid? treinamentoId, Guid? canalId);
        List<T> ListarPor(Guid? treinamentoId, Guid? canalId, Guid? compCanalId);
        List<T> ListarPorInativo(Guid? treinamentoId, Guid? canalId, Guid? compCanalId);
        List<T> ListarExpirados();
        List<T> ListarTodos();
        T ObterPor(Guid treinamentoCanalId);
        Boolean AlterarStatus(Guid treinamentoCanalId, int status);
       
    }
}
