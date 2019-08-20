using Microsoft.Xrm.Sdk;
using System;

namespace Intelbras.CRM2013.Application.Plugin.ibc_histdistrib
{
    public class ManagerPostEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            var historicoDistService = new Domain.Servicos.HistoricoDistribuidorService(context.OrganizationName, context.IsExecutingOffline, adminService);

            switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
            {
                case Domain.Enum.Plugin.MessageName.Create:
                    var entityTargetCreate = context.GetContextEntity();
                    var histDistCreate = entityTargetCreate.Parse<Domain.Model.HistoricoDistribuidor>(context.OrganizationName, context.IsExecutingOffline, adminService);

                    histDistCreate.Status = (int)Domain.Enum.HistoricoDistribuidor.Statecode.Ativo;
                    var historicDistService = new Domain.Servicos.HistoricoDistribuidorService(context.OrganizationName, context.IsExecutingOffline, adminService);
                    if (!histDistCreate.IntegrarNoPlugin || histDistCreate.IntegrarNoPlugin == null || histDistCreate.IntegrarNoPlugin.ToString().Equals(""))
                    {
                        historicoDistService.Integrar(histDistCreate);
                    }
                    
                    break;

                case Domain.Enum.Plugin.MessageName.Update:

                    Entity entidadeAlterada = (Entity)context.InputParameters["Target"];
                    Domain.Model.HistoricoDistribuidor historicoDistribuidor = entidadeAlterada.Parse<Domain.Model.HistoricoDistribuidor>(context.OrganizationName, context.IsExecutingOffline);

                    var entityMergeUpdate = context.GetContextEntity("postimagem");
                    var histDistUpdate = entityMergeUpdate.Parse<Domain.Model.HistoricoDistribuidor>(context.OrganizationName, context.IsExecutingOffline, adminService);
                    
                    histDistUpdate.IntegrarNoPlugin = historicoDistribuidor.IntegrarNoPlugin;

                    if (!histDistUpdate.IntegrarNoPlugin || histDistUpdate.IntegrarNoPlugin == null || histDistUpdate.IntegrarNoPlugin.ToString().Equals(""))
                    {
                        historicoDistService.Integrar(histDistUpdate);
                    }

                    break;
                case Domain.Enum.Plugin.MessageName.SetStateDynamicEntity:
                    if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                    {
                        var histDistServiceState = new Domain.Servicos.HistoricoDistribuidorService(context.OrganizationName, context.IsExecutingOffline, adminService);
                        var histDist = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.HistoricoDistribuidor>(context.OrganizationName, context.IsExecutingOffline, adminService);

                        var crmAdmin = SDKore.Configuration.ConfigurationManager.GetSettingValue("ID_CRMADMIN");

                        if (histDist.ModificadoPor.Id != new Guid(crmAdmin))
                            histDistServiceState.Integrar(histDist);
                    }

                    break;
            }
        }
    }
}