using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IValorDoServico<T> : IRepository<T>, IRepositoryBase
    {
        decimal ObterMaiorValorPor(Diagnostico servicoExecutado);
        decimal ObterMaiorValorPor(Ocorrencia ocorrencia);
    }
}