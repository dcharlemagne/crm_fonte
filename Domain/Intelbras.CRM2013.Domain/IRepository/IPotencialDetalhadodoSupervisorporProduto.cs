using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPotencialDetalhadodoSupervisorporProduto<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarDetalheProdutosPorMeta(Guid metaId, Guid supervisorId, Guid produtoId);
        DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<MetadaUnidade> lstMetas);
        List<T> Listar(Guid supervisorId, Guid produtoId, Guid trimestreId);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes? mes = null);
        List<PotencialdoSupervisorporProdutoDetalhado> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes mes);
        T Obter(int ano, int trimestre, int mes, Guid produtoId, Guid supervisorId, Guid potencialsupervisorprodutoId);
    }
}