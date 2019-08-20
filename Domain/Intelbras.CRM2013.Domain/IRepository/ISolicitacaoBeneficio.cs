using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ISolicitacaoBeneficio<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(Guid itbc_solicitacaodebeneficioid, int? stateCode);
        T ObterPor(ProdutosdaSolicitacao produtoSolicitacao, params string[] columns);
        List<T> ObterPorStatusPrice(int status);
        List<T> ListarAprovado(Guid beneficiocanalId, Guid canalId, Guid unidadenegocioId, Guid beneficioprograma);
        List<T> ListarPorBeneficioCanal(Guid beneficioCanalId);
        List<T> ListarPorBeneficioCanalEAjusteSaldo(Guid beneficioCanalId,Boolean ajusteSaldo);
        List<T> ListarPorBeneficioCanalEStatus(Guid beneficiocanalId, Guid beneficioPrograma, int status);
        Boolean AlterarStatus(Guid id, int stateCode, int statusCode);
        List<T> ListarDiferenteDeCanceladaEPagAnteriorAAtual();
    }
}
