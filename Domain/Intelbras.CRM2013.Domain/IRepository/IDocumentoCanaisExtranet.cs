using Intelbras.CRM2013.Domain.Enum;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IDocumentoCanaisExtranet<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid[] classificacoesId, Guid[] categoriasId, DocumentoCanaisExtranet.RazaoStatus razaoStatus, bool somenteVigente, bool todosCanais = false);
    }
}
