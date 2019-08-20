using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Application.Plugin.itbc_portfolio
{
    public class ManagerPreEvent : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {

            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));



            Entity Entidade = (Entity)context.InputParameters["Target"];

            Domain.Model.Portfolio portfolio = Entidade.Parse<Domain.Model.Portfolio>(context.OrganizationName, context.IsExecutingOffline);


            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    case Domain.Enum.Plugin.MessageName.Create:

                        new Intelbras.CRM2013.Domain.Servicos.PortfolioService(context.OrganizationName, context.IsExecutingOffline).VerificaDuplicidadeClassificadores(portfolio);
                        break;

                    case Domain.Enum.Plugin.MessageName.Update:

                        if (context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity)
                        {
                            var portfolioPre = ((Entity)context.PreEntityImages["imagem"]).Parse<Domain.Model.Portfolio>(context.OrganizationName, context.IsExecutingOffline);

                            if (portfolio.Tipo.HasValue == false)
                                portfolio.Tipo = portfolioPre.Tipo;

                            if (portfolio.UnidadeNegocio == null)
                                portfolio.UnidadeNegocio = new SDKore.DomainModel.Lookup(portfolioPre.UnidadeNegocio.Id, SDKore.Crm.Util.Utility.GetEntityName<Intelbras.CRM2013.Domain.Model.UnidadeNegocio>());

                            
                            if (portfolio.Classificacao == null)
                                portfolio.Classificacao = new SDKore.DomainModel.Lookup(portfolioPre.Classificacao.Id, SDKore.Crm.Util.Utility.GetEntityName<Intelbras.CRM2013.Domain.Model.Classificacao>()); 

                        }

                        new Intelbras.CRM2013.Domain.Servicos.PortfolioService(context.OrganizationName, context.IsExecutingOffline).VerificaDuplicidadeClassificadores(portfolio);

                        break;
                }
            }

        }
    }
}
