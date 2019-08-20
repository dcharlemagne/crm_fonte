using System;
using System.Collections.Generic;
using SDKore.DomainModel;
namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IProcesso<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid processoId);
        T ObterPor(Guid processoId);
        T ObterPorTipoDeSolicitacao(Guid TipoDeSolicitacaoId, Guid? BeneficioPrograma);
    }
}
