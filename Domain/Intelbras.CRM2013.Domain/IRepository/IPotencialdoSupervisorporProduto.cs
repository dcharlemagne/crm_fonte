using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPotencialdoSupervisorporProduto<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarProdutos(Guid potencialsubfamiliaId);
        List<T> ListarPor(Guid metaunidadeId, List<Guid> lstIdProdutos);
        List<T> ListarSupervisores(Guid metaunidadeId, List<Guid> lstIdSupervisor);
        List<PotencialdoSupervisorporProduto> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres trimestre);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres? trimestre = null);
        T Obter(Guid supervisorId, Guid produtoId, Guid trimestreId);
        T Obter(Guid supervisorId, Guid produtoId, Guid potencialdosupervisorporsubfamiliaId, int Trimestre);
        T Obter(Guid unidadenegocioId, Guid supervisorId, int ano, int trimestre, Guid segmentoId, Guid familiaId, Guid subfamiliaId, Guid produtoId);
        DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<MetadaUnidade> lstMetas);
    }
}