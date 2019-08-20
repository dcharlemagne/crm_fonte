using System;
using System.Collections.Generic;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ICanalVerde<T>:IRepository<T>,IRepositoryBase
    {
        T ObterPor(Guid itbc_canais_verdeid);
        T ObterParaCalculo(Guid canalId, Guid familiaProdId);
        List<T> ListarPorCanal(Guid canalGuid);
        List<T> ListarPorCanalTodos(Guid canalGuid);
        List<T> ListarPorSegmento(Guid segmentoGuid);
        List<T> ListarPorFamilia(Guid familiaGuid);
        Boolean AlterarStatus(Guid itbc_canais_verdeid, int status);
        void InativarMultiplos(List<T> collection, int status);
    }
}
