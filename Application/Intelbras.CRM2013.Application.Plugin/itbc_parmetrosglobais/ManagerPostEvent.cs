using System;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Application.Plugin.itbc_parmetrosglobais
{
    public class ManagerPostEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            {
                var repositoryService = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline, adminService);

                try
                {
                    Entity postImage = null;

                    switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                    {
                        case Domain.Enum.Plugin.MessageName.Update:
                        case Domain.Enum.Plugin.MessageName.Create:

                            if (!context.PostEntityImages.Contains("imagem"))
                            {
                                throw new InvalidPluginExecutionException("(CRM) É necessário registrar uma Post Image para esse plugin!");
                            }

                            postImage = (Entity)context.PostEntityImages["imagem"];
                            Domain.Model.ParametroGlobal parametroGlobalPostImage = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.ParametroGlobal>(context.OrganizationName, context.IsExecutingOffline, userService);
                            Domain.Integracao.MSG0167 msgParamGlobal = new Domain.Integracao.MSG0167(context.OrganizationName, context.IsExecutingOffline);
                            msgParamGlobal.Enviar(parametroGlobalPostImage);

                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(SDKore.Helper.Error.GetMessageError(ex));
                }
            }
        }
    }
}
