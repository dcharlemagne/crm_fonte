using System;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Application.Plugin.new_pagamento_servico
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
                Domain.Model.PagamentoServico pagamentoServico = new Domain.Model.PagamentoServico(context.OrganizationName, context.IsExecutingOffline);

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {

                    switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                    {
                        case Domain.Enum.Plugin.MessageName.Create:
                            entidade = (Entity)context.InputParameters["Target"];
                            pagamentoServico = entidade.Parse<Domain.Model.PagamentoServico>(context.OrganizationName, context.IsExecutingOffline);

                            break;

                        case Domain.Enum.Plugin.MessageName.Update:

                            if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                            {
                                entidade = (Entity)context.PostEntityImages["imagem"];
                                pagamentoServico = entidade.Parse<Domain.Model.PagamentoServico>(context.OrganizationName, context.IsExecutingOffline);
                            }

                            break;

                    }

                    if (!pagamentoServico.IntegrarNoPlugin)
                    {
                        new Domain.Servicos.PagamentoServicoService(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(pagamentoServico);
                    }

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