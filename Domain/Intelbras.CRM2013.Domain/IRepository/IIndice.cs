using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IIndice<T>:IRepository<T>,IRepositoryBase
    {
        List<T> ListarPor(Guid itbc_indiceid, Guid? itbc_tabeladefinanciamento);
        T ObterPor(String chave_integracao);
        bool AlterarStatus(Guid indiceID, int statecode, int statuscode);
        //
    }
}
