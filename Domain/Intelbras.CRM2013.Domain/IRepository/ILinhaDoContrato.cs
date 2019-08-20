using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ILinhaDoContrato<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Contrato contrato);
        T ObterPor(Contrato contrato);
        T ObterPor(Ocorrencia ocorrencia, params string[] columns);
    }
}
