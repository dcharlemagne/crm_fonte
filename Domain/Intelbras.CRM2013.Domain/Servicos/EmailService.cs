using Intelbras.CRM2013.Domain.Model;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class EmailService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public EmailService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public EmailService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public EmailService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Envia email para os endereços de email em destinatários.
        /// </summary>
        /// <param name="assunto"></param>
        /// <param name="corpoDoEmail"></param>
        /// <param name="destinatarios"></param>
        /// <param name="FileName"></param>
        public void EnviaEmailComAnexo(string assunto, string corpoDoEmail, string[] destinatarios, string fileName)
        {
            var email = new Email(this.RepositoryService.NomeDaOrganizacao, this.RepositoryService.IsOffline);

            Guid fromUserId;

            if (!Guid.TryParse(SDKore.Configuration.ConfigurationManager.GetSettingValue("Intelbras.Usuario.EnvioEmail", true), out fromUserId))
            {
                throw new ArgumentException("(CRM) O parâmetro 'Intelbras.Usuario.EnvioEmail' não foi possível converter em GUID.");
            }

            email.Assunto = assunto;
            email.De = new Lookup[1];
            email.De[0] = new Lookup(fromUserId, "systemuser");
            email.Mensagem = corpoDoEmail;
            email.ParaEnderecos = string.Join(";", destinatarios);

            this.AnexaArquivoNoEmail(ref email, fileName);

            this.RepositoryService.Email.EnviaEmailParaDestinatariosNaoCRM(email);
        }

        private void AnexaArquivoNoEmail(ref Email email, string fileName)
        {
            if (!File.Exists(fileName))
                throw new ArgumentException("(CRM) " + fileName + " não encontrado.");

            var stream = File.OpenRead(fileName);

            var byteData = new byte[stream.Length];

            stream.Read(byteData, 0, byteData.Length);

            // Encode the data using base64.
            var encodedData = Convert.ToBase64String(byteData);
            var extension = Path.GetExtension(fileName).ToLower();

            String mimeType = null;

            switch (extension)
            {
                case ".xls":
                    mimeType = "application/vnd.ms-excel";
                    break;
                case ".xlsx":
                    mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case ".txt":
                    mimeType = "text/plain";
                    break;
            }

            var anexo = new AnexoDeEmail
            {
                FileName = Path.GetFileName(fileName),
                MimeType = mimeType,
                BodyBase64 = encodedData,
            };

            if (email.Anexos == null)
                email.Anexos = new List<AnexoDeEmail>();

            email.Anexos.Add(anexo);
        }
        public List<Email> ListarPorReferenteA(Guid referenteA)
        {
            return RepositoryService.Email.ListarPorReferenteA(referenteA);
        }
        public Email ObterDataUltimoEmailEnviado(Guid ocorrencia)
        {
            return RepositoryService.Email.ObterDataUltimoEmailEnviado(ocorrencia);
        }
        #endregion
    }
}