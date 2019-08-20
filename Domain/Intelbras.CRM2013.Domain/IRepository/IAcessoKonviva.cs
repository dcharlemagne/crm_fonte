using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IAcessoKonviva<T> : IRepository<T>, IRepositoryBase
    {
        Boolean AlterarStatus(Guid acessoKonvivaId, int status);
        T ObterPor(Guid acessoKonvivaId);
        T ObterPorNome(String nome);
        T ObterPorContato(Guid contatoId, Domain.Enum.StateCode status);
        List<T> ListarPor(Guid contatoId);
        List<T> ListarPor(object[] dicionarioValores);
        List<T> ListarPorUnidade(Guid[] dicionarioValores);
        List<T> ListarPorCriterioDeParaContas(DeParaDeUnidadeDoKonviva objDePara, Guid idUnidadePadrao);
        List<T> ListarPorCriterioDeParaContatos(DeParaDeUnidadeDoKonviva objDePara, Guid idUnidadePadrao);
    }
}