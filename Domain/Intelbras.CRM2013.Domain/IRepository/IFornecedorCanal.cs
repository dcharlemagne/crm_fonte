using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IFornecedorCanal<T>: IRepository<T>,IRepositoryBase
    {
        List<T> ListarPor(Guid itbc_fornecedorcanalid);
        T ObterPor(Guid itbc_fornecedorcanalid);
        //List<T> ListarPor(String accountid);
        bool AlterarStatus(Guid itbc_estadoid, int status);
    }
}
