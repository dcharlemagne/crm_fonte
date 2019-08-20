using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using System.Diagnostics;
using Intelbras.Crm.Domain.Model.Ocorrencias;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Infrastructure.Dal;
using Tridea.Framework.DomainModel;
using Intelbras.Crm.Domain.Model.Enum;
using Microsoft.Crm.SdkTypeProxy;
using System.IO;
using Intelbras.Crm.Domain.Model.Clientes;
using Intelbras.Crm.Domain.Model.Usuarios;
using Intelbras.Crm.Domain.Services.GestaoSLA;


namespace Intelbras.Crm.Application.Plugin.contact
{
    public class PreCreate : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            DynamicEntity entity = null;

            try
            {
                var isDynamicEntity = (context.InputParameters.Properties.Contains("Target") && context.InputParameters.Properties["Target"] is DynamicEntity);
                if (!isDynamicEntity) return;
                
                    Organizacao organizacao = new Organizacao(context.OrganizationName);
                    entity = (DynamicEntity)context.InputParameters.Properties["Target"];

                    #region Valida Duplicidade de Login
                                    
                    if (!entity.Properties.Contains("new_login")) return;

                    string newLogin = entity.Properties["new_login"].ToString();

                    if (newLogin != "")
                    {
                        Contato contato = new Contato(organizacao);
                        contato = contato.PesquisarPorLogin(newLogin);
                        if (contato != null)
                            throw new InvalidPluginExecutionException("Esse contato não pode ser salvo. O login já existe no portal corporativo.");
                    }

                    #endregion
                
            }
            catch (InvalidPluginExecutionException exception)
            {
                throw exception;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", String.Format("Houve um problema ao executar o plugin 'email.PreCreate': Mensagem: {0} -- StackTrace: {1} \n--{2}", ex.Message, ex.StackTrace, ex.InnerException));
            }

        }
    }
}
