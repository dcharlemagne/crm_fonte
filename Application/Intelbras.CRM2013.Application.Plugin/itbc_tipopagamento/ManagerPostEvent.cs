using System;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Application.Plugin.itbc_tipopagamento
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
                Domain.Model.TipoPagamento tipoPagamento = new Domain.Model.TipoPagamento(context.OrganizationName, context.IsExecutingOffline);

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {

                    switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                    {
                        case Domain.Enum.Plugin.MessageName.Create:
                            entidade = (Entity)context.InputParameters["Target"];
                            tipoPagamento = entidade.Parse<Domain.Model.TipoPagamento>(context.OrganizationName, context.IsExecutingOffline);

                            break;

                        case Domain.Enum.Plugin.MessageName.Update:

                            if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                            {
                                entidade = (Entity)context.PostEntityImages["imagem"];
                                tipoPagamento = entidade.Parse<Domain.Model.TipoPagamento>(context.OrganizationName, context.IsExecutingOffline);
                            }

                            break;

                    }

                    new Domain.Servicos.TipoPagamentoService(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(tipoPagamento);

                }

            }
            catch (Exception ex)
            {
                trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), @"Modelo", DateTime.Now));
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

    }

}