using Microsoft.Xrm.Sdk;
using System;

namespace Intelbras.CRM2013.Application.Plugin.CustomerAddress
{
    public class ManagerPostEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            try
            {
                switch (context.GetMessageName())
                {
                    case PluginBase.MessageName.Create:

                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                        {
                            var entidade = context.GetContextEntity();
                            Domain.Model.Endereco endereco = entidade.Parse<Domain.Model.Endereco>(context.OrganizationName, context.IsExecutingOffline, adminService);

                            if (endereco.IntegrarNoPlugin)
                            {
                                string xmlResposta = new Domain.Servicos.EnderecoService(context.OrganizationName,
                                    context.IsExecutingOffline, adminService).IntegracaoBarramento(endereco);
                            }
                        }
                        break;
                    case PluginBase.MessageName.Update:

                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            var entityMerge = context.PostEntityImages["imagem"];
                            var enderecoMerge = ((Entity)entityMerge).Parse<Domain.Model.Endereco>(context.OrganizationName, context.IsExecutingOffline, adminService);

                            if (enderecoMerge.IntegrarNoPlugin)
                            {
                                string resposta = new Domain.Servicos.EnderecoService(context.OrganizationName, context.IsExecutingOffline, adminService).IntegracaoBarramento(enderecoMerge);
                            }
                        }

                        break;
                }
            } 
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
