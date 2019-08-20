using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IRelacionamentoCanal<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid? RelacionamentoDoCanalID);
        List<T> ListarPor(Guid contaId, Domain.Enum.Conta.StateCode? stateCode = null);
        T ObterPor(Guid RelacionamentoDoCanalID);
        T ObterPor(Guid guidCanal, Guid guidSupervisor, Guid guidKeyAccount, Guid guidAssistente, DateTime dtInicial, DateTime dtFinal);

        List<T> ListarPor(Guid guidCanal, Guid guidSupervisor, Guid guidKeyAccount, Guid guidAssistente, DateTime dtInicial, DateTime dtFinal);
        List<T> ListarPorKeyAccount(Guid guidCanal, Guid guidKeyAccount, DateTime dtAtual);
        Boolean AlterarStatus(Guid id, int stateCode, int statusCode);
        void AdicionarEquipe(Guid Equipe, Guid Supervisor);
        void RemoverEquipe(Guid Equipe, Guid Supervisor);
        T ObterPorSupervisor(Guid Canal, Guid Supervisor);
        T ObterPorAssistente(Guid Canal, Guid Assistente);
    }
}
