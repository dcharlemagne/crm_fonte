using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Application.Plugin.itbc_pausa_de_tarefa
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            switch (EnumConverter<MessageName>(context.MessageName.ToLower()))
            {
                #region Create

                case MessageName.Create:
                    break;
                #endregion

                #region Update
                case MessageName.Update:
                    break;

                #endregion

                #region SetStateDynamicEntity

                case MessageName.SetStateDynamicEntity:
                    EntityReference entity = (EntityReference)context.InputParameters["EntityMoniker"];
                    OptionSetValue state = (OptionSetValue)context.InputParameters["State"];
                    OptionSetValue status = (OptionSetValue)context.InputParameters["Status"];
                    IOrganizationService service = serviceFactory.CreateOrganizationService(null);


                    break;

                    #endregion
            }
        }
    }
}