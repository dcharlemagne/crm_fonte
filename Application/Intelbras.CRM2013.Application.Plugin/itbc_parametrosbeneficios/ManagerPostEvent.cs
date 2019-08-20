using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Application.Plugin.itbc_parametrosbeneficios
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
                            Domain.Model.ParametroBeneficio parametroBeneficioPostImage = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.ParametroBeneficio>(context.OrganizationName, context.IsExecutingOffline, userService);
                            Domain.Integracao.MSG0166 msgBenefCanal = new Domain.Integracao.MSG0166(context.OrganizationName, context.IsExecutingOffline);
                            msgBenefCanal.Enviar(parametroBeneficioPostImage);

                            break;
                        case Domain.Enum.Plugin.MessageName.SetStateDynamicEntity:
                            postImage = (Entity)context.PostEntityImages["imagem"];
                            parametroBeneficioPostImage = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.ParametroBeneficio>(context.OrganizationName, context.IsExecutingOffline, userService);
                            msgBenefCanal = new Domain.Integracao.MSG0166(context.OrganizationName, context.IsExecutingOffline);
                            msgBenefCanal.Enviar(parametroBeneficioPostImage);
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
