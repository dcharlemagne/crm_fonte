using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IAreaAtuacao<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(Guid areaAtuacaoId);

        T ObterPorCodigo(int codigoAreaAtuacao);

        List<T> ListarPorContato(Guid contatoId);
    }
}
