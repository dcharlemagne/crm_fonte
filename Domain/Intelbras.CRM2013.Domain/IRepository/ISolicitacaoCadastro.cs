using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ISolicitacaoCadastro<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPorConta(Guid itbc_solicitacaodecadastroid, DateTime? dtInicio, DateTime? dtFim, int? status);
        T ObterPor(Guid itbc_solicitacaodecadastroid);
        List<T> ListarPor(String SolicitacaoCadastroNome);
        bool AlterarStatus(Guid condicaoPitbc_solicitacaodecadastroidagamentoId, int state);
        
    }
}
