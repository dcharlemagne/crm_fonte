using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IAutorizacaoDePostagem<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarTodosAutorizacaoPostagemCorreios();
        List<T> ListarTodosAutorizacaoPostagemCorreios(int count, int page);
        List<T> PesquisarAutorizacaoPostagemCorreiosPor(string eTiketOuNumeroObjeto);
        List<T> ListarContratoAutorizacaoPostagemCorreiosPor(Ocorrencia ocorrencia);
    }
}
