using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IUnidadeNegocio<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid businessunitid);
        List<T> ListarPorConta(Guid itbc_accountid);
        List<T> ListarPorParticipaProgramaPci(bool participaProgramaPci, params string[] columns);
        List<T> ListarTodasParticipaProgramaPci(bool participaProgramaPci);
        List<T> ListarPor(String unidadeNegocioNome);
        T ObterPor(Guid unidadeNegocioCodigoId);
        T ObterPorChaveIntegracao(string itbc_chave_integracao);
        List<T> ObterPorChaveIntegracao(string[] conjChavesIntegracao);
        T ObterPor(int unidadeNegocioCodigo);
        T ObterPor(LinhaComercial linhaComercial);
        List<T> ListarTodos();
        List<T> ListarTodosChaveIntegracao();
        List<T> ListarUnidadesDeNegocioB2BPor(Domain.Model.Conta cliente);
        Guid ObterRelacionamentoUnidadeNegocioBenef(string itbc_chave_integracao);
        

    }
}