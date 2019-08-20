using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IConfiguracaoBeneficio<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPorProduto(Guid? produto);
        T ObterPor(Guid itbc_configuracaodebeneficioid);
        bool AlterarStatus(Guid ConfiguracaoBeneficioid, int statuscode);
    }
    }
