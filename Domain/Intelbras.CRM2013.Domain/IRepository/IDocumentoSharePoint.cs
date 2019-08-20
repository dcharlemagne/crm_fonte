using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IDocumentoSharePoint<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid sharepointdocumentid);
        List<T> ListarPorIdRegistro(Guid objetoRelativoId);
        T ObterPor(String urlRelativa);
        bool AlterarStatus(Guid sharepointdocumentid, int status);

        Boolean AlterarURL(T doc);

        List<T> ListarTodos();
    }
}
