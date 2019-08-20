using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;
using System;

namespace Intelbras.CRM2013.Application.Plugin.itbc_contasegmento
{
    public class ManagerPostEvent : IPlugin
    {
        private Object thisLock = new Object();

        public void Execute(IPluginExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            lock (thisLock)
            {
                var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(null);

                ContaSegmentoService contaSegmentoService = new ContaSegmentoService(context.OrganizationName, context.IsExecutingOffline, service);

                try
                {
                    trace.Trace(context.MessageName);

                    switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                    {
                        case Domain.Enum.Plugin.MessageName.Create:
                            var entidade = (Entity)context.InputParameters["Target"];

                            //verifica o campo Mais Verde
                            var contaSegmentoCreate = contaSegmentoService.Busca(entidade.Id);
                            contaSegmentoService.AtualizaSegmentos(contaSegmentoCreate);
                            break;

                        case Domain.Enum.Plugin.MessageName.Update:

                            if (context.PostEntityImages.Contains("image") && context.PostEntityImages["image"] is Entity)
                            {
                                //verifica o campo Mais Verde
                                var contaSegmentoUpdate = context.PostEntityImages["image"].Parse<ContaSegmento>(context.OrganizationName, context.IsExecutingOffline, service);
                                contaSegmentoService.AtualizaSegmentos(contaSegmentoUpdate);
                            }

                            break;
                    }
                }
                catch (Exception ex)
                {
                    string message = SDKore.Helper.Error.Handler(ex);

                    trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                    throw new InvalidPluginExecutionException(message, ex);
                }
            }
        }
    }
}
