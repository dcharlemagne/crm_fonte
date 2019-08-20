using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Application.Plugin.itbc_relacionamentodocanal
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
                var relacionamentoCanalService = new Intelbras.CRM2013.Domain.Servicos.RelacionamentoCanalService(context.OrganizationName, context.IsExecutingOffline, service);

                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    case Domain.Enum.Plugin.MessageName.Create:
                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                        {
                            var entidade = (Entity)context.InputParameters["Target"];
                            var RelacionamentoCanal = entidade.Parse<Domain.Model.RelacionamentoCanal>(context.OrganizationName, context.IsExecutingOffline);

                            //Se registro duplicado estoura exception
                            relacionamentoCanalService.VerificarRegistroDuplicado(RelacionamentoCanal);
                           
                            //Envia mensagem 0137
                            if (!RelacionamentoCanal.IntegrarNoPlugin)
                                relacionamentoCanalService.IntegracaoBarramento(RelacionamentoCanal);
                        }
                        break;

                    case Domain.Enum.Plugin.MessageName.Update:
                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            var RelCanalPre = ((Entity)context.PreEntityImages["imagem"]).Parse<Domain.Model.RelacionamentoCanal>(context.OrganizationName, context.IsExecutingOffline);

                            var RelCanalPost = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.RelacionamentoCanal>(context.OrganizationName, context.IsExecutingOffline);

                            //Envia mensagem 0137 caso não haja mudança de status
                            if  (RelCanalPre.Status == RelCanalPost.Status)
                                relacionamentoCanalService.IntegracaoBarramento(RelCanalPost);
                        }
                        break;

                    case Domain.Enum.Plugin.MessageName.Delete:

                        break;

                    #region SetStateDynamicEntity

                    case Domain.Enum.Plugin.MessageName.SetStateDynamicEntity:

                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            var RelCanalPost = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.RelacionamentoCanal>(context.OrganizationName, context.IsExecutingOffline);

                            //Envia mensagem 0137 caso haja mudança de status
                            relacionamentoCanalService.IntegracaoBarramento(RelCanalPost);
                        }
                        break;
                        #endregion
                }
            }
            catch (Exception ex)
            {
                string message = SDKore.Helper.Error.Handler(ex);

                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(message);
            }
        }
    }
}
