using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;
using SDKore.DomainModel;
using System.Reflection;

namespace Intelbras.CRM2013.Application.Plugin.product
{
    public class ManagerPreEvent : IPlugin
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
                    case Domain.Enum.Plugin.MessageName.Update:

                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                        {
                            Entity target = (Entity)context.InputParameters["Target"];
                            Entity preImage = (Entity)context.PreEntityImages["imagem"];
                            var produto = target.Parse<Domain.Model.Product>(context.OrganizationName, context.IsExecutingOffline);
                            var preProduto = preImage.Parse<Domain.Model.Product>(context.OrganizationName, context.IsExecutingOffline);

                            if (CustoAtualFoiModificado(produto, preProduto))
                            {
                                target.Attributes["itbc_dataultalteracaopvc"] = DateTime.UtcNow;
                            }

                            if(!target.Attributes.Contains("itbc_temmensagem") || target.GetAttributeValue<bool>("itbc_temmensagem") == false)
                            {
                                target.Attributes["itbc_mensagem"] = null;
                            }
                        }
                        break;
                    case Domain.Enum.Plugin.MessageName.Create:

                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                        {
                            Entity target = (Entity)context.InputParameters["Target"];

                            if (!target.Attributes.Contains("itbc_temmensagem") || target.GetAttributeValue<bool>("itbc_temmensagem") == false)
                            {
                                target.Attributes["itbc_mensagem"] = null;
                            }
                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));

                string message = SDKore.Helper.Error.Handler(ex);
                throw new InvalidPluginExecutionException(message, ex);
            }
        }

        private bool CustoAtualFoiModificado(Domain.Model.Product target, Domain.Model.Product pre)
        {
            if (target.CustoAtual.HasValue)
            {
                if (pre.CustoAtual.HasValue)
                {
                    if (target.CustoAtual.Value == pre.CustoAtual.Value)
                    {
                        return false;
                    }
                }

                return true;
            }
            else if (pre.CustoAtual.HasValue)
            {
                return true;
            }

            return false;
        }
    }
}
