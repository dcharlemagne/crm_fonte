using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;


namespace Intelbras.CRM2013.Application.Plugin.systemuser
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            try
            {
                var e = context.GetContextEntity();
                Domain.Model.Usuario usuario = e.Parse<Intelbras.CRM2013.Domain.Model.Usuario>(context.OrganizationName, context.IsExecutingOffline,userService);
                var ServiceUser = new UsuarioService(context.OrganizationName, context.IsExecutingOffline, userService);

                var entityTargetUpdate = (Entity)context.InputParameters["Target"];
                if (entityTargetUpdate.Attributes.Contains("itbc_codigodoassistcoml"))
                {
                    var codigodoassistcoml = entityTargetUpdate.Attributes["itbc_codigodoassistcoml"];

                    if (codigodoassistcoml != null)
                    {
                        if (ServiceUser.BuscaPorCodigoAssistente((int)codigodoassistcoml) != null)
                        {
                            if (ServiceUser.BuscaPorCodigoAssistente((int)codigodoassistcoml).CodigoAssistenteComercial != 0)
                            {
                                throw new ArgumentException("Ja existe um usuario com o mesmo Codigo Assistente Comercial. Favor informar outro codigo.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}