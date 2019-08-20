using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IMunicipio<T>:IRepository<T>,IRepositoryBase
    {
        List<T> ListarPor(string ChaveIntegracao);
        T ObterPor(Guid itbc_municipioid);
        T ObterPor(Guid itbc_estadoid, string itbc_name);
        T ObterPor(string uf, string itbc_name);
        T ObterPor(string chaveIntegracao);
        bool AlterarStatus(Guid itbc_estadoid, int status);
        Domain.ViewModels.IbgeViewModel ObterIbgeViewModelPor(int codigoIbge);

        //CRM4
        T ObterPor(int codigoIBGE);
        //T ObterPor(Guid itbc_estadoid, string cidadeNome);
        T ObterPorCep(string cep);
        List<T> ListarPor(Guid itbc_municipioid);
        List<T> ListarPor(List<Regional> listaRegionais);
        List<T> ListarCidadesPor(Regional regional);
        List<T> ListarPor(Estado uf);
        //CRM4
    }
}