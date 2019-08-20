using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;


namespace Intelbras.CRM2013.Application.Plugin.systemuser
{
    public class ManagerPostEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            try
            {
                var e = context.GetContextEntity();
                //Domain.Model.Usuario usuario = e.Parse<Intelbras.CRM2013.Domain.Model.Usuario>(context.OrganizationName, context.IsExecutingOffline,userService);
                var ServiceUser = new UsuarioService(context.OrganizationName, context.IsExecutingOffline, userService);

                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    #region SetStateDynamicEntity

                    case Domain.Enum.Plugin.MessageName.SetStateDynamicEntity:
                        {
                            var postImage = (Entity)context.PostEntityImages["imagem"];
                            var Image = context.GetContextEntity("imagem", false).Parse<Intelbras.CRM2013.Domain.Model.Usuario>(context.OrganizationName, context.IsExecutingOffline, userService);
                            
                            if (Image.IsDisabled)
                            {
                                postImage.Attributes["itbc_codigodoassistcoml"] = 0;
                                adminService.Update(postImage);
                            }
                        }

                        break;

                    #endregion
                }
            }
            catch (Exception ex)
            {
                //Trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), "itbc_solicitacaodebeneficio", DateTime.Now));
                //Trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}