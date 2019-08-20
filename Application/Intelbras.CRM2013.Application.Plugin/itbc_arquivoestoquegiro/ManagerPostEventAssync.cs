using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Application.Plugin.itbc_arquivoestoquegiro
{
    public class ManagerPostEventAssync : IPlugin
    {

        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);

            try
            {
                trace.Trace("MessageName: {0}", context.MessageName);

                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    case Domain.Enum.Plugin.MessageName.Create:

                        var entidade = (Entity)context.InputParameters["Target"];
                        Domain.Model.ArquivoDeEstoqueGiro ArquivoEstoqueGiro = entidade.Parse<Domain.Model.ArquivoDeEstoqueGiro>(context.OrganizationName, context.IsExecutingOffline);

                        if (ArquivoEstoqueGiro != null && !string.IsNullOrEmpty(ArquivoEstoqueGiro.Nome))
                        {
                            SharepointServices sharepointServices = new SharepointServices(context.OrganizationName, context.IsExecutingOffline, service);
                            sharepointServices.CriarDiretorio<Domain.Model.ArquivoDeEstoqueGiro>(ArquivoEstoqueGiro.Nome, ArquivoEstoqueGiro.ID.Value);
                            trace.Trace(sharepointServices.Trace.StringTrace.ToString());
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Intelbras.CRM2013.Util.Utilitario.TratarErro(ex);
                throw ex;
            }
        }

    }
}
