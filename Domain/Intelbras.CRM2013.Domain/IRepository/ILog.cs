using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ILog<T> : IRepository<T>, IRepositoryBase
    {
        /*void ExcluirEmMassa();
        string GetValue(string entidade, string campoPesquisado, string campoRetornado, string valorCampoPesquisado);*/

        List<T> ListarLogDo(Domain.Model.Log diagnostico);
    }
}
