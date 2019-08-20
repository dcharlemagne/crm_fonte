using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Application.Plugin.Contact
{
    public class ManagerPostEventAssync : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
            {
                case Domain.Enum.Plugin.MessageName.Create:
                case Domain.Enum.Plugin.MessageName.Update:

                    var entidade = (Entity)context.InputParameters["Target"];
                    Contato Contato = entidade.Parse<Contato>(context.OrganizationName, context.IsExecutingOffline);

                    #region Email portal fidelidade
                    (new RepositoryService(context.OrganizationName, context.IsExecutingOffline)).Contato.EnviaEmailAcessoPortalCorporativo(Contato);
                    #endregion
                    break;
            }
        }
    }
}
