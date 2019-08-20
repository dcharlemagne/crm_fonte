using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IEquipe<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid equipeId);
        List<T> ListarPorNome(String nomeEquipe);
        T ObterPor(Guid equipeId);
        void RemoverPerfilAcesso(Guid equipeId,Guid perfilId);
        void AdicionarPerfilAcesso(Guid equipeId, Guid perfilId);
    }
}
