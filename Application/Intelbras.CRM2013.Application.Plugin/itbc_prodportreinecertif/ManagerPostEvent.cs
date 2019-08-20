using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;



namespace Intelbras.CRM2013.Application.Plugin.itbc_prodportreinecertif
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
                            Domain.Model.ProdutoTreinamento ProdTreinamento = entidade.Parse<Domain.Model.ProdutoTreinamento>(context.OrganizationName, context.IsExecutingOffline, service);

                            //Criação de Treinamento
                            // açao transportada para monitoramento diario
                            //new Intelbras.CRM2013.Domain.Servicos.PortfolioService(context.OrganizationName, context.IsExecutingOffline).CriaTreinamentoeCertificacaoDoCanal(ProdTreinamento);

                        }

                        break;

                    case Domain.Enum.Plugin.MessageName.Update:

                        
                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            var ProdTreinamento = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.ProdutoTreinamento>(context.OrganizationName, context.IsExecutingOffline, service);

                            //Atualização de Treinamento
                            // açao transportada para monitoramento diario
                            //new Intelbras.CRM2013.Domain.Servicos.PortfolioService(context.OrganizationName, context.IsExecutingOffline).AtualizaTreinamentoeCertificacaoDoCanal(ProdTreinamento);
                        }

                        break;
                   

                    case Domain.Enum.Plugin.MessageName.Delete:

                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity
                            && context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity)
                        {

                        }
                        break;
                }

            }            catch (Exception ex)
            {
                trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), @"Categoria", DateTime.Now));
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

    }

}