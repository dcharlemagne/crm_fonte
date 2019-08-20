using System;
using System.Collections.Generic;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPerfil<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid? classificacaoId, Guid? unidadenegocioId, Guid? categoriaId, Boolean? Exclusividade);
        List<T> ListarPorConfigurado(Guid? classificacaoId, Guid? unidadenegocioId, Guid? categoriaId, Boolean? Exclusividade);
        T ObterPor(Guid PerfilId);
    }
}
