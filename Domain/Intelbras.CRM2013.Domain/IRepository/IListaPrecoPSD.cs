using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IListaPrecoPSD<T>:IRepository<T>,IRepositoryBase
    {
        List<T> ListarPor(Guid? estadoId);
        List<T> ListarPor(Guid itbc_businessunit, DateTime? itbc_Data_Inicio, DateTime? itbc_Data_Fim);
        List<T> ListarPor(Guid unidadeNegocioId, List<Guid> lstEstados,Guid? listaGuid, DateTime dtInicio, DateTime dtFim);
        T ObterPor(Guid itbc_psdldid);
        T ObterPor(Guid? EstadoId,Guid? UnidadeNegocio);
        List<T> ListarPor(Guid unidadeNegocioId, Guid? registroCorrenteId);

    }
}
