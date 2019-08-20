using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Application.Plugin.itbc_classificacao
{
    public class ManagerPostEvent : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);

            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            try
            {
                Entity entidade = new Entity();
                Domain.Model.Classificacao classificacao = new Domain.Model.Classificacao(context.OrganizationName, context.IsExecutingOffline);

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {

                    switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                    {
                        case Domain.Enum.Plugin.MessageName.Create:
                            entidade = (Entity)context.InputParameters["Target"];
                            classificacao = entidade.Parse<Domain.Model.Classificacao>(context.OrganizationName, context.IsExecutingOffline);

                            break;

                        case Domain.Enum.Plugin.MessageName.Update:

                            if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                            {
                                entidade = (Entity)context.PostEntityImages["imagem"];
                                classificacao = entidade.Parse<Domain.Model.Classificacao>(context.OrganizationName, context.IsExecutingOffline);
                            }

                            break;

                    }

                    new Domain.Servicos.ClassificacaoService(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(classificacao);

                }

            }
            catch (Exception ex)
            {
                trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), @"Classificação", DateTime.Now));
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

    }

}