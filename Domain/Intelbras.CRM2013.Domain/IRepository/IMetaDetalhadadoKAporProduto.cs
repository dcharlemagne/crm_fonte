using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using System.Data;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IMetaDetalhadadoKAporProduto<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarDetalheProdutosKaRepresentante(Guid metaId);
        List<T> ListarDetalheProdutosKaRepresentante(Guid metaId, Guid? karepresentanteId, Guid? produtoId);
        List<T> ListarPor(Guid potencialdokaporprodutoId);
        List<T> ListarTodosDetalheProdutosKaRepresentante(Guid metaId, int? pagina, int? contagem, Guid unidadeId, int ano);
        List<T> ListarPorAnoUnidadeNegocio(Guid unidadenegocioId, int ano, params string[] columns);
        T Obter(Guid unidadenegocioId, Guid KARepresentanteId, int ano, int trimestre, int mes, Guid produtoId, Guid potencialkaprodutoId);
        DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<MetadaUnidade> lstMetas);
    }
}

