using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Sellout;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.ViewModels;
using Intelbras.CRM2013.Domain.Integracao;

namespace Intelbras.CRM2013.Application.Plugin.itbc_produtodacondiodepagamento
{
    public class ManagerPostEventAsy : IPlugin
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
                Domain.Model.ProdutoCondicaoPagamento produtoCondicaoPagamento = new Domain.Model.ProdutoCondicaoPagamento(context.OrganizationName, context.IsExecutingOffline,serviceProvider);

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {

                    switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                    {
                        case Domain.Enum.Plugin.MessageName.Create:
                            entidade = (Entity)context.InputParameters["Target"];
                            produtoCondicaoPagamento = entidade.Parse<Domain.Model.ProdutoCondicaoPagamento>(context.OrganizationName, context.IsExecutingOffline,serviceProvider);

                            break;

                        case Domain.Enum.Plugin.MessageName.Update:

                            if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                            {
                                entidade = (Entity)context.PostEntityImages["imagem"];
                                produtoCondicaoPagamento = entidade.Parse<Domain.Model.ProdutoCondicaoPagamento>(context.OrganizationName, context.IsExecutingOffline,serviceProvider);
                            }

                            break;

                        case Domain.Enum.Plugin.MessageName.Delete:

                            if (context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity)
                            {
                                entidade = (Entity)context.PreEntityImages["imagem"];
                                produtoCondicaoPagamento = entidade.Parse<Domain.Model.ProdutoCondicaoPagamento>(context.OrganizationName, context.IsExecutingOffline, serviceProvider);
                            }

                            break;

                    }

                    new Domain.Servicos.ProdutoCondicaoPagamentoService(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(produtoCondicaoPagamento);

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
