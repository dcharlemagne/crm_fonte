using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPrivilegio<T> : IRepository<T>, IRepositoryBase
    {
        List<T> PesquisarPrivilegioPor(Usuario usuario);
        Boolean PermissaoIncluirEm(Usuario usuario, String entidade);
    }
}
