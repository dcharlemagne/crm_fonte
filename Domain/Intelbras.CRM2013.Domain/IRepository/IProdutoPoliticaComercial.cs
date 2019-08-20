using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IProdutoPoliticaComercial<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid politicaComercialId);
        T ObterPor(Guid politicaComercialId, Guid produtoId, int quantidade);
        List<T> ListarPor(Guid politicaComercialId, Guid produtoId, Guid? produtoPoliticaComercialId, DateTime dtInicio, DateTime dtFim, int qntInicial, int qntFinal);
        List<T> ListarPorProduto(Guid produtoId);
        T ObterPor(Guid produtoPoliticaComercialId);
        void AlterarStatus(Guid produtoPoliticaComercialId, int status);
    }
}