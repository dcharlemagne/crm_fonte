using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPoliticaComercial<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPorTipoNull();
        List<T> ListarPor(Guid estabelecimentoId, Guid? politicaComercialId, Guid? unidadeNegocioId, Guid? classificacaoId, Guid? categoriaId, int TipoPolitica, DateTime DtInicio, DateTime DtFim);
        T ObterPorPoliticaEspecifica(Guid estabelecimentoId, Guid? politicaComercialId, Guid? canalId, Guid? unidadeNegocioId, Guid? classificacaoId, Guid? categoriaId);
        List<T> ListarPor(Guid? estabelecimentoId, Guid? politicaComercialId, Guid? canalId, Guid? unidadeNegocioId, Guid? classificacaoId, Guid? categoriaId);
        T ObterPor(Guid politicaComercialId);
        List<T> ListarPoliticaComercialEspecificaTodos();
        List<T> ListarPor(Guid? PoliticaComercialId, int TipoPolitica, int AplicarPoliticaPara, Guid estabelecimentoId, Guid unidadeNegocioId, List<Guid> Canais, DateTime dtInicio, DateTime dtFim);
        List<T> ListarPorEstado(Guid? PoliticaComercialId,int TipoPolitica, int AplicarPoliticaPara, Guid estabelecimentoId, Guid unidadeNegocioId,Guid Classificacao,Guid Categoria, List<Guid> Estados,DateTime dtInicio,DateTime dtFim);
        List<T> ListarPorEstado(Guid Estado);
        List<T> ListarPorCanal(Guid Canal);
        List<T> ListarPoliticasComClassificacao();
        void CriarAssociacaoCanal(Guid PoliticaID, Guid CanalID);
        void CriarAssociacaoEstado(Guid PoliticaID, Guid EstadoID);
    }
}