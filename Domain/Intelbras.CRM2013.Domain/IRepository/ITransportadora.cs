using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ITransportadora<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid id);
        T ObterPor(String CodTransportadora);
        T ObterPor(Guid itbc_codigodatransportadora);
        bool AlterarStatus(Guid transportadora, int status);

        T ObterPor(Model.Conta cliente);
    }
}
