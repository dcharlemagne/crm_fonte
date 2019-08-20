using Microsoft.Xrm.Sdk;
using System;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.ViewModels;
using SDKore.Helper;

namespace Intelbras.CRM2013.Application.Plugin.Email
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            try
            {
                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    #region Create
                    case Domain.Enum.Plugin.MessageName.Create:

                        var parameter = (Entity)context.InputParameters["Target"];
                        if (!parameter.Attributes["subject"].ToString().Contains("ALERTA"))
                        {
                            var email = parameter.Parse<Domain.Model.Email>(context.OrganizationName, context.IsExecutingOffline, adminService);
                            ModeloEmailIsol(ref email);
                            if (email.MensagemErro == "ALTERADO")
                            {
                                parameter.Attributes.Remove("description");
                                parameter.Attributes.Add("description", email.Mensagem);
                            }

                            if (string.IsNullOrEmpty(email.Assunto))
                            {
                                parameter.Attributes.Remove("subject");
                                parameter.Attributes.Add("subject", "{Sem assunto}");
                            }
                        }

                        break;
                        #endregion
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        protected void ModeloEmailIsol(ref Domain.Model.Email email)
        {
            if (email.ReferenteA != null && email.ReferenteA.Type == "incident" && !string.IsNullOrEmpty(email.Mensagem))
            {

                if (email.Mensagem.Contains("{link_ocorrencia_isol}"))
                {
                    string link = string.Format("<a href=\"{0}\">Clique Aqui</a>", SDKore.Configuration.ConfigurationManager.GetSettingValue("LINK_ISOL_FECHAMENTO") + email.ReferenteA.Id.ToString());
                    email.Mensagem = email.Mensagem.Replace("{link_ocorrencia_isol}", link);
                    email.MensagemErro = "ALTERADO";
                }

                if (email.Mensagem.Contains("{link_ocorrencia_impressao}"))
                {
                    string link = string.Format("<a href=\"{0}\">Clique Aqui</a>", SDKore.Configuration.ConfigurationManager.GetSettingValue("LINK_ISOL_OS") + email.ReferenteA.Id.ToString());
                    email.Mensagem = email.Mensagem.Replace("{link_ocorrencia_impressao}", link);
                    email.MensagemErro = "ALTERADO";
                }

                if (email.Mensagem.Contains("{valor_linha_contrato}"))
                {
                    var ocorrencia = new Ocorrencia() { Id = email.ReferenteA.Id };
                    var linhaDeContrato = (new Domain.Servicos.RepositoryService()).LinhaDoContrato.ObterPor(ocorrencia, "new_valor_pago");

                    string valorPago = (linhaDeContrato != null && linhaDeContrato.PrecoPago.HasValue) ? linhaDeContrato.PrecoPago.Value.ToString("F") : "0,00";

                    email.Mensagem = email.Mensagem.Replace("{valor_linha_contrato}", valorPago);
                    email.MensagemErro = "ALTERADO";
                }
            }
        }
    }
}