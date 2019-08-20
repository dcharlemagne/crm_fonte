using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Application.Plugin.itbc_regiaogeo
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
                Domain.Model.Itbc_regiaogeo regiaoGeografica = new Domain.Model.Itbc_regiaogeo(context.OrganizationName, context.IsExecutingOffline);

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Intelbras.CRM2013.Domain.Integracao.MSG0008 integ = new Domain.Integracao.MSG0008(context.OrganizationName, context.IsExecutingOffline); 
                    switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                    {
                        case Domain.Enum.Plugin.MessageName.Create:
                            entidade = (Entity)context.InputParameters["Target"];
                            regiaoGeografica = entidade.Parse<Domain.Model.Itbc_regiaogeo>(context.OrganizationName, context.IsExecutingOffline);
                            integ.EnviarRegiaoGeo(regiaoGeografica);
                            
                            break;

                        case Domain.Enum.Plugin.MessageName.Update:

                            if (context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity)
                            {
                                #region Popula o objeto com pos-image

                                entidade = (Entity)context.PostEntityImages["imagem"];
                                regiaoGeografica = entidade.Parse<Domain.Model.Itbc_regiaogeo>(context.OrganizationName, context.IsExecutingOffline);
                                integ.EnviarRegiaoGeo(regiaoGeografica);
                                #endregion
                            }

                            break;

                    }

                    //new Domain.Servicos.EnderecoServices(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(regiaoGeografica);

                }

            }
            catch (Exception ex)
            {
                trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), @"Nivel Pós Venda", DateTime.Now));
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

    }

}