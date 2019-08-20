using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Application.Plugin.itbc_cnae
{
    public class ManagerPostEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            var cnaeService = new Domain.Servicos.CnaeService(context.OrganizationName, context.IsExecutingOffline, adminService);

            switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
            {
                case Domain.Enum.Plugin.MessageName.Create:
                    var entityTargetCreate = context.GetContextEntity();
                    var cnae = entityTargetCreate.Parse<Domain.Model.CNAE>(context.OrganizationName, context.IsExecutingOffline, adminService);

                    cnaeService.Integrar(cnae);
                    break;

                case Domain.Enum.Plugin.MessageName.Update:
                    var entityMergeUpdate = context.GetContextEntity("postimagem");
                    var cnaeUpdate = entityMergeUpdate.Parse<Domain.Model.CNAE>(context.OrganizationName, context.IsExecutingOffline, adminService);

                    cnaeService.Integrar(cnaeUpdate);
                    break;
            }
        }
    }
}
