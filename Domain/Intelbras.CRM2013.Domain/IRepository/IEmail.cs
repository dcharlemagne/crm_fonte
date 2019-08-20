using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IEmail<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid email);
        void EnviarEmail(Guid emailId);
        void EnviaEmailParaDestinatariosNaoCRM(Email email);
        Boolean AlterarStatus(Guid emailid, int statuscode);
        void EnviaEmailComLogdeRotinas(string textoEmail, string assunto, string nomeArquivo, string emailTo);
        void EnviaEmailExternos(string textoEmail, string assunto, string nomeArquivo, string emailTo, string remetente);
        void EnviaEmailComUsuariosDesativados(string textoEmail, string assunto);
        List<T> ListarPorReferenteA(Guid referenteA);
        Email ObterDataUltimoEmailEnviado(Guid ocorrencia);

        List<T> ListarPor(string nomeEntidade, string nomeCampo, string valorCampo);

    }
}
