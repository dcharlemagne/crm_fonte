using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IColaboradorTreinadoCertificado<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid contatoId);
        List<T> ListarPorCanal(Guid accountId);
        List<T> ListarPorCanalTreinamentosAprovadosValidos(Guid accountId);
        List<T> ListarPor(Guid canalId, Guid treinamentoId);
        List<T> ListarExpirados();
        T ObterPor(Guid colaboradorTreinadoId);
        T ObterPor(Int32 idMatricula);
        Boolean AlterarStatus(Guid treinamentoid, int status);
    }
}
