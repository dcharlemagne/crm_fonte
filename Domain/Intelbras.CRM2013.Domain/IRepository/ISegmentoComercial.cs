using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using System.Data;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ISegmentoComercial<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(Guid itbc_segmentocomercialid);
        List<T> ListarSegmentoPorConta(string contaId);
        List<T> ListarTodos();
        bool AlterarStatus(Guid segmentoId, int status);
        T ObterPorCodigo(string codigoSeg);
        void AssociarSegmentoComercial(List<SegmentoComercial> segmentoComercial, Guid conta);
        void DesassociarSegmentoComercial(List<SegmentoComercial> segmentoComercial, Guid conta);
    }
}
