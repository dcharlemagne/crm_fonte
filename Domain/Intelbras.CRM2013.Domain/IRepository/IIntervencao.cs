using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IIntervencao<T> : IRepository<T>, IRepositoryBase
    {
        T EmAnalisePor(Ocorrencia ocorrencia);
        List<T> ListarPor(Ocorrencia ocorrencia);
        List<T> ListarPor(Ocorrencia ocorrencia, int status);
        List<T> ListarPorOcorrenciaId(Guid ocorrenciaId);
        T ObterPor(Ocorrencia ocorrencia);
        bool OcorrenciaTemIntervencaoAtiva(Ocorrencia ocorrencia);
    }
}
