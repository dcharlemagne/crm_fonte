using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ILinhaCorteDistribuidor<T>:IRepository<T>,IRepositoryBase
    {
        List<T> ListarPor(List<Guid> unidadeNegocioId, Guid? estadoId);
        T ListarPorUnidade(Guid unidadeNegocioId, Guid? estadoId, Guid classificacaoId);
        T ObterPor(Guid linhadecorteid);
        List<T> ListarPort(Guid classificacaoid, Guid? estadoId, int? capitalOuInterior, Guid? categoriaId);
        DataTable FaturamentoUltimoTrimestreRevendaDW(int ano, int trimestreAnterior, Guid canalid);
        DataTable FaturamentoGlobalSemestreDW(int ano1, int ano2, string trimestres, string canalid);
        DataTable FaturamentoUnidadeSemestreDW(int ano1, int ano2, string trimestres, string canalid);
        DataTable FaturamentoGlobalTrimestreDW(int ano, int trimestreAnterior, string canalid);
        DataTable FaturamentoUnidadeTrimestreDW(int ano, int trimestreAnterior, string canalid);
        DataTable FaturamentoLinhasCorteDWTrimestre(string canalid, string x);



    }
}
