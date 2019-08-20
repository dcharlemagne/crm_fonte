using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IValorDoServicoPorPosto<T> : IRepository<T>, IRepositoryBase
    {
        decimal ObterMaiorValorPor(Model.Conta cliente);
        decimal ObterMaiorValorPor(Model.Conta cliente, Product produto);
        decimal ObterMaiorValorPorLinhaComercialDoProduto(Model.Conta cliente, Product produto);
        decimal ObterMaiorValorPor(Product produto);
    }
}