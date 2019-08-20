using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{

    /// <summary>
    /// Autor: Marcelo Ferreira de Láias (MSBS Tridea)
    /// Data: 05/09/2011
    /// Descrição: 
    /// </summary>
    public interface IHistoricoDePostagem<T> : IRepository<T>, IRepositoryBase
    {
        List<T> PesquisarHistoricoDePostagemPor(string eTiket);
        List<T> PesquisarHistoricoDePostagemPor(Ocorrencia ocorrencia);
        List<T> PesquisarHistoricoDePostagemPor(Model.Conta cliente);
        T PesquisarHistoricoOcorrencia(Ocorrencia ocorrencia, string numeroObjeto, string tipoEvento, string statusEvento, DateTime DataHora);
    }
}