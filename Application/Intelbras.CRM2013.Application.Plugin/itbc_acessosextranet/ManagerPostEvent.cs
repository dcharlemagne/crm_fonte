using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Application.Plugin.itbc_acessosextranet
{
    public class ManagerPostEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            var e = context.GetContextEntity();

            AcessoExtranet mAcessoExtranet = e.Parse<AcessoExtranet>(context.OrganizationName, context.IsExecutingOffline, adminService);
            AcessoExtranetService sAcessoExtranetService = new AcessoExtranetService(context.OrganizationName, context.IsExecutingOffline, adminService);

            switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
            {
                #region Create

                case Domain.Enum.Plugin.MessageName.Create:

                    sAcessoExtranetService.IntegracaoBarramento(mAcessoExtranet);
                    break;

                #endregion

                #region Update

                case Domain.Enum.Plugin.MessageName.Update:
                    var acessoExtranet = ((Entity)context.PostEntityImages["imagem"])
                        .Parse<AcessoExtranet>(context.OrganizationName, context.IsExecutingOffline, userService);

                    sAcessoExtranetService.IntegracaoBarramento(acessoExtranet);
                    break;

                    #endregion
            }
        }
    }
}