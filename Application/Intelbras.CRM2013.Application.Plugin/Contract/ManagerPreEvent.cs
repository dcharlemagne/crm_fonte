using Microsoft.Xrm.Sdk;
using System;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.ViewModels;
using SDKore.Helper;

namespace Intelbras.CRM2013.Application.Plugin.Contract
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            try
            {
                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    #region Create
                    case Domain.Enum.Plugin.MessageName.Create:

                        break;
                    #endregion

                    #region Update
                    case Domain.Enum.Plugin.MessageName.Update:
                        /*var targetUpdate = (Entity)context.InputParameters["Target"];
                        if (targetUpdate.Contains("new_data_termino_real"))
                        {
                            targetUpdate.Attributes.Add("expireson", targetUpdate.Attributes["new_data_termino_real"]);
                        }*/
                        break;
                        #endregion
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}