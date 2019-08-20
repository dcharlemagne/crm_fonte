using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IFamiliaComercial<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(string codigoFamilia);
        List<T> ListarPor(string codigoFamiliaInicial, string codigoFamiliaFinal);
        T ObterPor(Guid familiaComercialId);
        T ObterPor(string codigoFamiliaComercial);
        bool AlterarStatus(Guid familiaMaterialId, int statuscode);

        //CRM4
        T ObterFamiliaComercialPor(Product produto);
        //List<FamiliaComercial> ListarTodas(params string[] columns);
        //CRM4
    }
}