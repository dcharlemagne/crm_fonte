using Microsoft.Xrm.Sdk;
using SDKore.Helper;

namespace Intelbras.CRM2013.Application.Plugin.itbc_cnae
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            var cnaeService = new Domain.Servicos.CnaeService(context.OrganizationName, context.IsExecutingOffline, adminService);

            switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
            {
                case Domain.Enum.Plugin.MessageName.Create:
                    var entityTargetCreate = context.GetContextEntity();
                    var cnae = entityTargetCreate.Parse<Domain.Model.CNAE>(context.OrganizationName, context.IsExecutingOffline, adminService);

                    PreencheNome(ref cnae, ref entityTargetCreate);
                    break;

                case Domain.Enum.Plugin.MessageName.Update:
                    var entityTargetUpdate = context.GetContextEntity();
                    var entityMergeUpdate = context.GetContextEntityMerge("imagem");

                    var cnaeUpdate = entityMergeUpdate.Parse<Domain.Model.CNAE>(context.OrganizationName, context.IsExecutingOffline, adminService);

                    PreencheNome(ref cnaeUpdate, ref entityTargetUpdate);
                    break;
            }
        }

        private void PreencheNome(ref Domain.Model.CNAE cnae, ref Entity target)
        {
            if (target.Attributes.Contains("itbc_name"))
            {
                target.Attributes.Remove("itbc_name");
            }

            string nome = (cnae.SubClasse + " - " + cnae.Denominacao).Truncate(100);

            cnae.Nome = nome;
            target.Attributes.Add("itbc_name", nome);
        }
    }
}
