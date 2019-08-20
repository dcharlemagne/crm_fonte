using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ICanaldeVenda<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid itbc_canaldevendaid);
        T ObterPor(Guid canaldevendaId);
        T ObterPor(int itbc_codigo_venda);
        bool AlterarStatus(Guid itbc_canaldevendaid, int status);

        //CRM4
        T ObterPor(LinhaComercial linhaComercial);
        //CRM4
    }
}