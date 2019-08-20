using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ISharePointSite<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid sharepointsiteid);
        T ObterPor(Guid sharepointsiteid);
        T ObterPor();
        bool AlterarStatus(Guid sharepointsiteid, int status);

        void AssociarDiretorioAoRegistroCRM(string urlCompleta, string nomeEntidade, string nomeRegistro, Guid idRegistro);
    }
}
