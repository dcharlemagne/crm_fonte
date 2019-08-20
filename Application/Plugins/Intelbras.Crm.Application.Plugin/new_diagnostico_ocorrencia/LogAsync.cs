using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Services;

namespace Intelbras.Crm.Application.Plugin.new_diagnostico_ocorrencia
{
    public class LogAsync : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            DateTime inicioExecucao = DateTime.Now;

            DomainService.Organizacao = new Organizacao(context.OrganizationName);
            DynamicEntity entity = null;

            Log log = new Log(DomainService.Organizacao);
            log.Nome = "Log de Alteração";
            log.Acao = this.NomeDaAcao(context.MessageName);

            try
            {
                PluginHelper.LogEmArquivo(context, "INICIO;", inicioExecucao.ToString(), "");
                switch (context.MessageName)
                {
                    case MessageName.Create:
                    case MessageName.Update:
                        if (!context.InputParameters.Properties.Contains("Target")
                            && !(context.InputParameters.Properties["Target"] is DynamicEntity)) return;

                        entity = context.InputParameters.Properties["Target"] as DynamicEntity;
                        log.DiagnosticoId = PluginHelper.GetEntityId(context);
                        break;

                    case MessageName.Assign:
                        if (!context.PostEntityImages.Contains("Image")
                           && !(context.PostEntityImages["Image"] is DynamicEntity)) return;

                        entity = context.PostEntityImages["Image"] as DynamicEntity;
                        log.DiagnosticoId = PluginHelper.GetEntityId(context);
                        break;

                    case MessageName.Delete:
                        if (!context.PreEntityImages.Contains("Image")
                           && !(context.PreEntityImages["Image"] is DynamicEntity)) return;
                        entity = context.PreEntityImages["Image"] as DynamicEntity;
                        break;
                }

                if (entity == null) throw new InvalidPluginExecutionException("Nenhuma DynamicEntity encontrada!");


                // Quem alterou
                DomainService.RepositoryUsuario.Colunas = new string[] { "fullname" };
                Usuario usuario = DomainService.RepositoryUsuario.Retrieve(context.InitiatingUserId);

                log.Alteracoes = this.MensageDeAlteracoes(entity);
                log.Alteracoes += string.Format("\n\nAlterado por: {0} \nRegistro: {1}", usuario.NomeCompleto, PluginHelper.GetEntityId(context));

                DomainService.RepositoryLog.Create(log);
                PluginHelper.LogEmArquivo(context, "FIM;", inicioExecucao.ToString(), DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                LogService.GravaLog(ex, TipoDeLog.PluginNew_diagnostico_ocorrencia, "LogAsync - " + context.MessageName);
                PluginHelper.LogEmArquivo(context, "ERRO;", inicioExecucao.ToString(), DateTime.Now.ToString());
                throw new InvalidPluginExecutionException(ex.Message, ex);
            }
        }

        private string GetValueFromProperty(Microsoft.Crm.Sdk.Property inputProperty)
        {
            Type propType = inputProperty.GetType();
            string propValue = string.Empty;


            if (propType == typeof(Microsoft.Crm.Sdk.StringProperty))
            {
                propValue = ((Microsoft.Crm.Sdk.StringProperty)inputProperty).Value;
            }
            else if (propType == typeof(Microsoft.Crm.Sdk.CustomerProperty))
            {
                var crmCustomer = ((Microsoft.Crm.Sdk.CustomerProperty)inputProperty).Value;
                if (!crmCustomer.IsNull || !crmCustomer.IsNullSpecified)
                    propValue = crmCustomer.Value.ToString();
            }
            else if (propType == typeof(Microsoft.Crm.Sdk.CrmBooleanProperty))
            {
                var crmBoolean = ((Microsoft.Crm.Sdk.CrmBooleanProperty)inputProperty).Value;
                if (!crmBoolean.IsNull || !crmBoolean.IsNullSpecified)
                    propValue = crmBoolean.Value.ToString();
            }
            else if (propType == typeof(Microsoft.Crm.Sdk.CrmMoneyProperty))
            {
                propValue = ((Microsoft.Crm.Sdk.CrmMoneyProperty)inputProperty).Value.Value.ToString();
            }
            else if (propType == typeof(Microsoft.Crm.Sdk.CrmDateTimeProperty))
            {
                string myValue;
                myValue = ((Microsoft.Crm.Sdk.CrmDateTimeProperty)inputProperty).Value.Value.ToString();
                propValue = Convert.ToDateTime(myValue).GetDateTimeFormats()[3];
            }
            else if (propType == typeof(Microsoft.Crm.Sdk.CrmDecimalProperty))
            {
                var crmDecimal = ((Microsoft.Crm.Sdk.CrmDecimalProperty)inputProperty).Value;
                if (!crmDecimal.IsNull || !crmDecimal.IsNullSpecified)
                    propValue = crmDecimal.Value.ToString();
            }
            else if (propType == typeof(Microsoft.Crm.Sdk.CrmFloatProperty))
            {
                var crmFloat = ((Microsoft.Crm.Sdk.CrmFloatProperty)inputProperty).Value;
                if (!crmFloat.IsNull || !crmFloat.IsNullSpecified)
                    propValue = crmFloat.Value.ToString();
            }
            else if (propType == typeof(Microsoft.Crm.Sdk.CrmNumberProperty))
            {
                var crmNumber = ((Microsoft.Crm.Sdk.CrmNumberProperty)inputProperty).Value;
                if (!crmNumber.IsNull || !crmNumber.IsNullSpecified)
                    propValue = crmNumber.Value.ToString();
            }
            else if (propType == typeof(Microsoft.Crm.Sdk.LookupProperty))
            {
                if (inputProperty.Name.ToString() != "owningbusinessunit")
                    propValue = ((Microsoft.Crm.Sdk.LookupProperty)inputProperty).Value.Value.ToString();
            }
            else if (propType == typeof(Microsoft.Crm.Sdk.OwnerProperty))
                propValue = ((Microsoft.Crm.Sdk.OwnerProperty)inputProperty).Value.Value.ToString();

            else if (propType == typeof(Microsoft.Crm.Sdk.StatusProperty))
            {
                propValue = ((Microsoft.Crm.Sdk.StatusProperty)inputProperty).Value.Value.ToString();
            }
            else if (propType == typeof(Microsoft.Crm.Sdk.PicklistProperty))
            {
                var crmPicklist = ((Microsoft.Crm.Sdk.PicklistProperty)inputProperty).Value;
                if(!crmPicklist.IsNull || !crmPicklist.IsNullSpecified)
                propValue = crmPicklist.name;
            }
            else if (propType == typeof(Microsoft.Crm.Sdk.KeyProperty))
                propValue = ((Microsoft.Crm.Sdk.KeyProperty)inputProperty).Value.Value.ToString();

            return propValue;
        }

        private string MensageDeAlteracoes(DynamicEntity entity)
        {
            var listaLabel = DomainService.RepositoryCadastro.ListarLabel(entity.Name);
            StringBuilder mensagem = new StringBuilder();
            string label = string.Empty;


            foreach (var item in entity.Properties)
                if (listaLabel.TryGetValue(item.Name, out label))
                    mensagem.AppendFormat("{0}: {1}\n", label, this.GetValueFromProperty(item));
                else
                    mensagem.AppendFormat("O campo {0} não foi encontrado\n", item.Name);

            return mensagem.ToString();
        }

        private string NomeDaAcao(string messageName)
        {
            switch (messageName)
            {
                case MessageName.Create: return "Criação";
                case MessageName.Update: return "Alteração";
                case MessageName.Assign: return "Proprietário";
                case MessageName.Delete: return "Exclusão";
                default: return string.Empty;
            }
        }
    }
}
