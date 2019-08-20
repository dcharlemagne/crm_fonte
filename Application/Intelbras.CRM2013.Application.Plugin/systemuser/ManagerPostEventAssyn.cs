using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;


namespace Intelbras.CRM2013.Application.Plugin.systemuser
{
    public class ManagerPostEventAssyn : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            try
            {
                var e = context.GetContextEntity();
                Domain.Model.Usuario usuario = e.Parse<Intelbras.CRM2013.Domain.Model.Usuario>(context.OrganizationName, context.IsExecutingOffline,userService);
                var ServiceUser = new UsuarioService(context.OrganizationName, context.IsExecutingOffline, userService);

                switch (EnumConverter<MessageName>(context.MessageName.ToLower()))
                {
                    case MessageName.Update:
                        {
                            #region
                            switch ((Stage)context.Stage)
                            {
                                case Stage.PostOperation:
                                    var imagemPre = context.GetContextEntity("imagem", true).Parse<Intelbras.CRM2013.Domain.Model.Usuario>(context.OrganizationName, context.IsExecutingOffline, userService);
                                    var imagemPos = context.GetContextEntity("imagem", false).Parse<Intelbras.CRM2013.Domain.Model.Usuario>(context.OrganizationName, context.IsExecutingOffline, userService);

                                    new Intelbras.CRM2013.Domain.Servicos.PortfoliodoKeyAccountRepresentantesService(context.OrganizationName, context.IsExecutingOffline, userService).AtualizarCodigosAssistenteSupervisor(imagemPre, imagemPos);


                                    break;
                            }
                            #endregion
                        }
                        break;
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