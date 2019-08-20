using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ISolicitacaoXUnidades<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(Guid itbc_solicitacaodebeneficioid);
        List<T> ListarPor(Guid itbc_solicitacaodebeneficioid);        
    }
}
