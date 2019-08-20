using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Application.Plugin.itbc_regiao_atuacao
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

                            var regiaoDeAtuacao = entidade.Parse<Domain.Model.RegiaoDeAtuacao>(context.OrganizationName, context.IsExecutingOffline, service);

                            string lstResposta = new Domain.Servicos.RegiaoDeAtuacaoService(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(regiaoDeAtuacao);
                        }

                        break;
                    #endregion

                    #region Update
                    case Domain.Enum.Plugin.MessageName.Update:

                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            var regiaoDeAtuacao = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.RegiaoDeAtuacao>(context.OrganizationName, context.IsExecutingOffline, service);

                            string lstResposta = new Domain.Servicos.RegiaoDeAtuacaoService(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(regiaoDeAtuacao);
                        }

                        break;

                    #endregion

                    #region SetStateDynamicEntity
                    case Domain.Enum.Plugin.MessageName.SetStateDynamicEntity:

                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            var regiaoDeAtuacao = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.RegiaoDeAtuacao>(context.OrganizationName, context.IsExecutingOffline, service);

                            string lstResposta = new Domain.Servicos.RegiaoDeAtuacaoService(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(regiaoDeAtuacao);
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