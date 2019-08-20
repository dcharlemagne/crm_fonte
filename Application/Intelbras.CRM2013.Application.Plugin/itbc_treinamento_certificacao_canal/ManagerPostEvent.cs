using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Application.Plugin.itbc_treinamento_certificacao_canal
{
    public class ManagerPostEvent : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);

            try
            {
                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    case Domain.Enum.Plugin.MessageName.Create:

                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                        {
                            var entidade = (Entity)context.InputParameters["Target"];
                            Domain.Model.TreinamentoCanal treinaCanal = entidade.Parse<Domain.Model.TreinamentoCanal>(context.OrganizationName, context.IsExecutingOffline, service);
                            // açao transportada para monitoramento diario
                            //new Domain.Servicos.TreinamentoService(context.OrganizationName, context.IsExecutingOffline, service).VerificaCumprimento(treinaCanal);
                        }
                        
                        break;

                    case Domain.Enum.Plugin.MessageName.Update:

                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            var treinaCanal = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.TreinamentoCanal>(context.OrganizationName, context.IsExecutingOffline, service);
                            // açao transportada para monitoramento diario
                            //new Domain.Servicos.TreinamentoService(context.OrganizationName, context.IsExecutingOffline, service).VerificaCumprimento(treinaCanal);
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), "itbc_treinamento_certificacao_canal", DateTime.Now));
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
