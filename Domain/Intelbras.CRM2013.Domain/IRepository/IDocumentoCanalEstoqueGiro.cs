using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IDocumentoCanalEstoqueGiro<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid? accountid, DateTime? DataCriacao, string URL, bool? SomenteArquivosNovos, DateTime? DataInicial, DateTime? DataFinal);
        T ObterPor(Guid accountid);
    }
}
