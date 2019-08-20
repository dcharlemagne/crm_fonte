using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IEstabelecimento<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid itbc_estabelecimentoid);
        T ObterPor(Guid itbc_estabelecimentoid);
        T ObterPor(int itbc_codigo_estabelecimento);
        bool AlterarStatus(Guid itbc_estabelecimentoid, int status);
        List<T> ListarTodos(params string[] columns);

        //CRM4
        T ObterPor(LinhaComercial linhaComercial);
        T ObterPor(Pedido pedido);
        List<T> ListarB2B();
        List<T> ListarEstabelecimentosPor(TabelaDePreco tabelaDePreco);
        //CRM4
    }
}

