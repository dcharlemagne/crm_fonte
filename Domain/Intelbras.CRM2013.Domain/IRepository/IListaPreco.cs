using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IListaPreco<T>:IRepository<T>,IRepositoryBase
    {
        List<T> ListarPor(Guid pricelevelid);
        List<T> ListarPor(Domain.Enum.ListaPreco.Tipo tipo, Guid? estadoId);
        T ObterPor(Guid pricelevelid);
        List<T> ListarPor(Domain.Enum.ListaPreco.Tipo tipo, Guid? estadoId, Guid? UnidadeNegocio);
        T ObterPor(Domain.Enum.ListaPreco.Tipo tipo, Guid? estadoId,Guid? UnidadeNegocio);
        List<T> ListarPor(String nomeListaPreco);
        List<T> ListarPor(Guid unidadeNegocioId, List<Guid> lstEstados, Guid? listaGuid, DateTime dtInicio, DateTime dtFim);
    }
}
