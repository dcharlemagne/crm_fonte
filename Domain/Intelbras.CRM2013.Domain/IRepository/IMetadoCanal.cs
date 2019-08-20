using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IMetadoCanal<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(Guid metaunidadetrimestreId, Guid canalId, int ano, int trimestre);
        T ObterPor(Guid unidadeNegocioId, Guid canalId, int ano);
        T ObterPor(Guid UnidNeg, int trimestre, Guid canalId, int ano, params string[] columns);
        T ObterPorCodigo(string chaveIntegracaoUnidadeNegocio, Domain.Enum.OrcamentodaUnidade.Trimestres trimestre, string codigoEmitenteCanal, int ano, params string[] columns);
        List<T> ListarPor(Guid metadotrimestreid);
        List<T> Listar(Guid metaunidadeId);
        List<T> Listar(Guid metaunidadeId, List<Guid> lstIdCanal);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano);
        List<ViewModels.ModeloMetaDetalhadaClienteViewModel> ListarModeloMetaDetalhada(Guid unidadeNegocioId, int ano);
        List<Domain.ViewModels.ModeloMetaResumidaClienteViewModel> ListarModeloMetaResumida(Guid unidadeNegocioId, int ano);
        List<MetadoCanal> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano);
        DataTable ListarMetaCanalDW(int ano, int trimestre, List<MetadaUnidade> lstMetasUnidade);
    }
}