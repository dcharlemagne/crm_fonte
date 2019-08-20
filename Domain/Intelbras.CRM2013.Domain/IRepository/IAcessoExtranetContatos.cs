using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IAcessoExtranetContatos<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid itbc_acessosextranetcontatosid);
        List<T> ListarPorContato(Guid contatoId);
        List<T> ListarAcessosAtivosPorContato(Guid contatoId);
        T ObterPor(Guid itbc_acessosextranetcontatosid);
        bool AlterarStatus(Guid itbc_portadorid, int status);
        List<T> ListarPor(Guid canalId, Guid contatoId);
        List<T> ListarPorCanal(Guid canalId);
        List<T> ListarTodos();
        Guid VerificarEmail(string email);
    }
}