using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IEstruturaAtendimento<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid itbc_estrutura_atendimentoid);
        T ObterPor(Guid itbc_estrutura_atendimentoid);
        bool AlterarStatus(Guid itbc_estrutura_atendimentoid, int status);
    }
}
