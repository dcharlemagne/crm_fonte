using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Application.Plugin.itbc_psdid
{
    public class ManagerPostEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            {
                var ServiceListaPSD = new Domain.Servicos.ListaPSDService(context.OrganizationName, context.IsExecutingOffline, adminService);

                try
                {
                    Entity _target = null;
                    Entity postImage = null;
                    switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                    {
                        case Domain.Enum.Plugin.MessageName.Create:

                            break;
                        case Domain.Enum.Plugin.MessageName.Update:
                            break;
                        case Domain.Enum.Plugin.MessageName.SetStateDynamicEntity:
                            postImage = (Entity)context.PostEntityImages["imagem"];

                            if (postImage.Contains("statuscode"))
                            {
                                if (postImage.GetAttributeValue<OptionSetValue>("statuscode").Value == (int)Domain.Enum.ListaPrecoPSDPPPSCF.StatusIntegracao.IntegracaoPendente ||
                                    postImage.GetAttributeValue<OptionSetValue>("statecode").Value == (int)Domain.Enum.StateCode.Inativo)

                                {
                                    new ListaPSDService(context.OrganizationName, context.IsExecutingOffline).IntegracaoBarramento(postImage.Parse<Domain.Model.ListaPrecoPSDPPPSCF>(context.OrganizationName, context.IsExecutingOffline));
                                    postImage["statuscode"] = new OptionSetValue((int)Domain.Enum.ListaPrecoPSDPPPSCF.StatusIntegracao.Integrado);
                                    postImage["itbc_datahoraintegracao"] = DateTime.UtcNow;

                                    if (postImage.GetAttributeValue<OptionSetValue>("statecode").Value != (int)Domain.Enum.StateCode.Inativo)
                                        adminService.Update(postImage);
                                }
                            }
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
