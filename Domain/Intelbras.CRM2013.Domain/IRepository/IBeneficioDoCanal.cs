using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IBeneficioDoCanal<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid accountid);
        T ObterPor(Guid itbc_acessosextranetcontatosid);
        T ObterPor(Guid beneficioPrograma, Guid unidadeNegocio, Guid canal);
        List<T> ListarPor(Guid canalId, Guid unidadeNegocioId);
        List<T> ListarPorConta(Guid canalId);
        List<T> ListarPorBeneficioProg(Guid beneficioprogId);
        List<T> ListarPorContaUnidadeNegocio(Guid canalId, Guid? unidadeNegocioId);
        List<T> ListarPorContaUnidadeNegocioESaldo(Guid canalId, Guid unidadeNegocioId);
        List<T> ListarPorContaUnidadeNegocioPlanilha();
        Boolean AtualizarStatus(Guid beneficioID, int stateCode, int statusCode);
        List<T> ListarPor(List<Guid> BeneficiosProg, Guid Canal, Guid UnidadeNegocios, Guid? Categoria);
        List<T> ListarComSaldoAtivos(Guid canalId);
    }
}
