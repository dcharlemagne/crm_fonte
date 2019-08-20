using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IFamiliaProduto<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(String codigoFamiliaId);
        T ObterPor(Guid familiaprodid);
        List<T> ListarPor(Guid unidadenegocioId);
        List<T> ListarPorSegmento(Guid segmentoId, bool filtrarCanaisVerdes, Guid? canalId, string[] notInList);
        bool AlterarStatus(Guid familiaProduto, int status);
        List<T> ListarPor(string codigoFamiliaInicial, string codigoFamiliaFinal);
        T ObterPor(string codigofamilia);
    }
}
