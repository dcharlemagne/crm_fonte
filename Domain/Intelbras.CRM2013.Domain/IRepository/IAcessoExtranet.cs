using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IAcessoExtranet<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid itbc_tipodeacesso);
        T ObterPor(Guid acessosextranetid);
        T ObterPor(Guid classificacaoid, Guid categoriaid, string Cargo);

        T ObterAdesao();
    }
}