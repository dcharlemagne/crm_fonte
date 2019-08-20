using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IUtil<T> : IRepository<T>, IRepositoryBase
    {
        bool MudarProprietarioRegistro(T objProprietario, Guid idProprietarioNovo, T entidadeDestino, Guid idEntidadeDestino);

        bool MudarProprietarioRegistro(string tipoProprietario, Guid idProprietarioNovo, string entidadeDestino, Guid idEntidadeDestino);

        string ObterNomeEntidade(T classe);
    }
}
