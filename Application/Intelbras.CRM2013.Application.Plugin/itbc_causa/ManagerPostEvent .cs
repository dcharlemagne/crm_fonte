using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;
using SDKore.Helper;
using System;

namespace Intelbras.CRM2013.Application.Plugin.itbc_causa
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
                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    #region Create
                    case Domain.Enum.Plugin.MessageName.Create:

                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                        {
                            var entidade = (Entity)context.InputParameters["Target"];

                            var obj = entidade.Parse<Domain.Model.Causa>(context.OrganizationName, context.IsExecutingOffline, service);

                            if (obj.Codigo != null)
                            {
                                string lstResposta = new Domain.Servicos.CausaService(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(obj);
                            }
                        }

                        break;
                    #endregion

                    #region Update
                    case Domain.Enum.Plugin.MessageName.Update:

                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            var obj = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.Causa>(context.OrganizationName, context.IsExecutingOffline, service);

                            if (obj.Codigo != null)
                            {
                                string lstResposta = new Domain.Servicos.CausaService(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(obj);
                            }
                        }

                        break;

                    #endregion

                    #region SetStateDynamicEntity
                    case Domain.Enum.Plugin.MessageName.SetStateDynamicEntity:

                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            var obj = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.Causa>(context.OrganizationName, context.IsExecutingOffline, service);

                            if (obj.Codigo != null)
                            {
                                string lstResposta = new Domain.Servicos.CausaService(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(obj);
                            }
                        }
                        break;
                        #endregion
                }
            }
            catch (Exception ex)
            {
                //trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), "Account", DateTime.Now));
                //trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

    }
}