using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IEstado<T> : IRepository<T>, IRepositoryBase
    {
        //List<T> ListarPor(Guid accountid);
        T ObterPor(Guid itbc_estadoid);
        T ObterPor(String nome);
        List<T> ListarPor(String ChaveIntegracao);
        List<T> ListarPor(ListaPrecoPSDPPPSCF listaPrecoPSDPPPSCF);
        List<T> ListarPorSigla(String siglaUF);
        List<T> ListarPor(Pais pais);
        bool AlterarStatus(Guid itbc_estadoid, int status);
        T PesquisarUfPor(string sigla, Pais pais);
        List<T> ListarUf(Guid paisId);
    }
}
