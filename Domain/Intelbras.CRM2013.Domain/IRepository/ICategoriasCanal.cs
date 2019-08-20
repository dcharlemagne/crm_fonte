using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ICategoriasCanal<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Conta canal);
        List<T> ListarPor(Guid? canalId, Guid? unidadenegocioId, Guid? classificacaoId, Guid? subclassificacaoId, Guid? CategoriaId);
        List<T> ListarPorUnidadeNegocio(Guid unidadenegocioId, Domain.Enum.Conta.ParticipaDoPrograma? participanteDoPrograma, bool? unidadeNegocioAtivo);
        T ObterPor(Guid categoriaCanalId, Guid? canalId);
        T ObterPor(Guid categoriaCanalId);
        T ObterPor(Domain.Model.Conta conta, Domain.Model.UnidadeNegocio unidade);
        bool AtualizarStatus(Guid categoriaCanalId, int stateCode, int statusCode);
    }
}
