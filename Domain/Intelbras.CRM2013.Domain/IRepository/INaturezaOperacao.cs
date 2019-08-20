using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface INaturezaOperacao<T>:IRepository<T>,IRepositoryBase
    {
        List<T> ListarPor(Guid itbc_natureza_operacaoid);
        T ObterPor(Guid itbc_natureza_operacaoid);
        List<T> ListarTodos();
        T ObterPor(String itbc_codigo_natureza_operacao);
        List<T> ObterPor(String[] conjCodigosNatureza);
        bool AlterarStatus(Guid itbc_natureza_operacaoid, int status);
    }
}
