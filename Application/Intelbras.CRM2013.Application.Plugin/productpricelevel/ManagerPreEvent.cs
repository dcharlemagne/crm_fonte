using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Application.Plugin.productpricelevel
{
    public class ManagerPreEvent : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {

            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);
            var ServiceListaPreco = new Intelbras.CRM2013.Domain.Servicos.ListaPrecoService(context.OrganizationName, context.IsExecutingOffline, service);

            try
            {

                var entidade = ((Entity)context.InputParameters["Target"]).Parse<Domain.Model.ItemListaPreco>(context.OrganizationName, context.IsExecutingOffline, service);

                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    case Domain.Enum.Plugin.MessageName.Create:

                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                        {
                            ServiceListaPreco.VerificaBUItemXListaDePreco(entidade);
                        }
                        break;

                    case Domain.Enum.Plugin.MessageName.Update:

                        if (context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity)
                        {
                            var entidadePre = ((Entity)context.PreEntityImages["imagem"]).Parse<Domain.Model.ItemListaPreco>(context.OrganizationName, context.IsExecutingOffline);

                            if (entidade.ListaPrecos == null)
                                entidade.ListaPrecos = new SDKore.DomainModel.Lookup(entidadePre.ListaPrecos.Id, SDKore.Crm.Util.Utility.GetEntityName<Intelbras.CRM2013.Domain.Model.ListaPreco>());

                            if (entidade.ProdutoID == null)
                                entidade.ProdutoID = new SDKore.DomainModel.Lookup(entidadePre.ProdutoID.Id, SDKore.Crm.Util.Utility.GetEntityName<Intelbras.CRM2013.Domain.Model.Product>());

                            ServiceListaPreco.VerificaBUItemXListaDePreco(entidade);
                        }
                        break;
                }


            }
            catch (Exception ex)
            {
                trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), "Product Price Level", DateTime.Now));
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
