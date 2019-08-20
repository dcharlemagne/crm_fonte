using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPais<T>:IRepository<T>,IRepositoryBase
    {
        List<T> ListarPor(String ChaveIntegracao);
         List<T> ListarTodos();
        T ObterPor(Guid itbc_paisid);
        bool AlterarStatus(Guid itbc_estadoid, int status);
        T PesquisarPaisPor(string nome);
        T PesquisarPaisPor(Guid ufId);
    }
}
