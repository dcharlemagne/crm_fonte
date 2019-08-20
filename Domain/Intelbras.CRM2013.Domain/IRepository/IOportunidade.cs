using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IOportunidade<T> : IRepository<T>, IRepositoryBase
    {
        //List<T> ListarPor(Guid itbc_origemid);
        void CriarAnotacaoParaUmLead(Guid leadId, Anotacao anotacao);
        List<T> ListarPor(Revenda revenda);

        T BuscarPor(ClientePotencial cliente);
    }
}
