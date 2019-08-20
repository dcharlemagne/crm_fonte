using System;
using System.Collections.Generic;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IMensagem<T>:IRepository<T>,IRepositoryBase
    {
        List<T> ListarPor(Guid itbc_mensagemid);
        T ObterPor(Guid itbc_mensagemid);
        T ObterPor(int codigoMensagem);
        bool AlterarStatus(Guid id, int status);
    }
}
