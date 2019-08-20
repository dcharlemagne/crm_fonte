using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IImportacaoAssistenciaTecnica<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarImportacoesPendentes();
      
    }
}
