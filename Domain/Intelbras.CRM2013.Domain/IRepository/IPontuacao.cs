using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPontuacao<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ObterListaCompletaVigenciaValida();
        List<T> ObterPor(Guid produtoId, DateTime inicio, DateTime? fim, Lookup pais, Lookup estado, Lookup distribuidor, Guid? pontuacaoId);
        T ObterPor(Guid produto);
    }

}
