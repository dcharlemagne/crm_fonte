using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Application.Plugin.itbc_posvenda
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
                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    case Domain.Enum.Plugin.MessageName.Create:
                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                            IntegracaoBarramento(context, service, (Entity)context.InputParameters["Target"]);

                        break;
                    case Domain.Enum.Plugin.MessageName.Update:
                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                            IntegracaoBarramento(context, service, (Entity)context.PostEntityImages["imagem"]);

                        break;
                }
            }
            catch (Exception ex)
            {
                trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), @"Nivel Pós Venda", DateTime.Now));
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        private void IntegracaoBarramento(IPluginExecutionContext context, IOrganizationService service, Entity entidade)
        {
            NivelPosVenda nivelPosVenda = entidade.Parse<NivelPosVenda>(context.OrganizationName, context.IsExecutingOffline, service);
            new Domain.Servicos.NivelPosVendaService(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(nivelPosVenda);
        }
    }
}