using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IUsuario<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid userId);
        List<T> ListarPor(String usuario);
        List<T> ListarPorUsuarioDesabilitado(Guid userId);
        T ObterPor(Guid accountid);
        T ObterPor(string login);
        T ObterPorCodigoSupervisorEMS(String itbc_codigo_supervisor);
        void AlterarStatus(Guid usuarioId, int state, int status);
        T ObterPorCodigoAssistente(int itbc_codigodoassistcoml);
        List<T> ListarSupervisoresPor(Guid unidadeId);
        List<T> ListarSupervisoresPorMeta(Guid metaId);
        List<T> ListarSupervisoresPor(Guid unidadeId, List<Guid> lstIdSupervisor);
        T ObterSupervisorPorRepresentanteKA(Guid KARepresentanteId);
        DataTable ListarUsuarioSemAcessar40Dias();
        T ObterUsuarioPorMatricula(string matricula);

        T BuscarProprietario(string entidadeRelacionada, string parametroIdEntidadeRelacionada, Guid idEntidadeRelacionada);

        //CRM4
        void AlterarStatus(Guid usuarioId, bool ativar);
        T ObterPor(string login, string nomeDeDominio);
        List<T> ListarPorFamiliaComercial(Product produto);
        //CRM4
    }
}
