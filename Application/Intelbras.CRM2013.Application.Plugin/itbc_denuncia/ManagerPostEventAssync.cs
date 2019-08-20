using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Application.Plugin.itbc_denuncia
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
                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    case Domain.Enum.Plugin.MessageName.Create:

                        var entidade = (Entity)context.InputParameters["Target"];
                        var denuncia = entidade.Parse<Domain.Model.Denuncia>(context.OrganizationName, context.IsExecutingOffline);

                        if (denuncia != null)
                        {
                            new SharepointServices(context.OrganizationName, context.IsExecutingOffline, service)
                                .CriarDiretorio<Domain.Model.Denuncia>("Denuncia", denuncia.ID.Value);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
