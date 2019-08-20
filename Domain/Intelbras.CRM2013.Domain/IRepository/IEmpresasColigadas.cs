using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IEmpresasColigadas<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid accountid);
        List<T> ListarPor(String nome);
        T ObterPor(Guid accountid);
        T ObterPor(String cnpj);
        bool AlterarStatus(Guid itbc_empresas_coligadasid, int statecode,int statuscode);
        bool AlterarProprietario(Guid proprietario, string TipoProprietario,Guid EmpresasColigadas);
    }
}
