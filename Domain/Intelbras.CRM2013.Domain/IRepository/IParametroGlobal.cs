using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IParametroGlobal<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPorCodigoTipoParametroGlobal(int Codigo);
        T ObterPor(int tipoParametro, Guid? unidadeNegocioId, Guid? classificacaoId, Guid? categoriaId, Guid? nivelPosVendaId, Guid? compromissoId, Guid? beneficioId,int? parametrizar);
        List<T> ListarPor(int tipoParametro, Guid? unidadeNegocioId, Guid? classificacaoId, Guid? categoriaId, Guid? nivelPosVendaId, Guid? compromissoId, Guid? beneficioId);

    }

}
