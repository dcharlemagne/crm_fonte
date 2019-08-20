using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.Application.Plugin.salesorderdetail
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
                    case Domain.Enum.Plugin.MessageName.Create:

                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                        {
                            Entity target = (Entity)context.InputParameters["Target"];

                            if (target.LogicalName == "salesorderdetail")
                            {
                                if (target.Contains("itbc_valor_total_item"))
                                {
                                    if (target.Contains("extendedamount"))
                                    {
                                        target.Attributes.Remove("extendedamount");
                                    }

                                    target.Attributes.Add("extendedamount", target.Attributes["itbc_valor_total_item"]);
                                }

                                try
                                {
                                    ColumnSet columns = new ColumnSet();
                                    Entity e = service.Retrieve(target.LogicalName, target.Id, new ColumnSet("quantity", "priceperunit", "salesorderispricelocked"));

                                    decimal total = 0;
                                    total = total + ((decimal)e["quantity"] * ((Money)e["priceperunit"]).Value);

                                    if (target.Contains("baseamount"))
                                    {
                                        target.Attributes.Remove("baseamount");
                                    }

                                    target.Attributes.Add("baseamount", new Money(total));
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.Write(ex.Message);
                                }

                                return;
                            }
                            else
                            {
                                return;
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
    }
}
