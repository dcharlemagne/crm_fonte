using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ISegmento<T> : IRepository<T>, IRepositoryBase
    {
        //List<T> ListarPor(Guid accountid);
        T ObterPor(Guid seguimentoId);
        T ObterPor(string itbc_codigo_segmento);
        List<T> ListarPor(String codigoSegmento);
        List<T> ListarPor(Guid codigoUnidadeNegocio);
        List<T> ListarPorContaSegmento(Guid contaId);
        List<T> ListarTodos();
        List<T> ListarCanaisVerdes();
        bool AlterarStatus(Guid itbc_estadoid, int status);
    }
}
