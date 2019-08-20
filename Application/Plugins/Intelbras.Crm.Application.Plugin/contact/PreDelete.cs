using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Model;
using System.Diagnostics;
using System.Web.Services.Protocols;
using Intelbras.Crm.Domain.Services;

namespace Intelbras.Crm.Application.Plugin.contact
{
    public class PreDelete : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            /***
             * Não permite exlcuir contatos com acesso ao portal corporativo,
             * o usuario do portal fica sem os seus relacionamentos com permissõs.
             ***/
            try
            {
                string login = string.Empty;

                DynamicEntity preContact = (DynamicEntity)context.PreEntityImages["contact"];

                if (preContact.Properties.Contains("new_login"))
                    login = preContact.Properties["new_login"].ToString();

                if (!String.IsNullOrEmpty(login))
                    throw new InvalidPluginExecutionException("Este registro não pode ser excluido: O contato é um usuário no portal Coporativo.");
            }
            catch (InvalidPluginExecutionException ex) { throw ex; } 
            catch (Exception ex)
            {
                LogService.GravaLog(ex, TipoDeLog.PluginContact);
                throw new InvalidPluginExecutionException("Ocorreu um erro inesperado, a operação foi cancelada!");
            }
        }
    }
}
