using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using System.Xml.Linq;
using System.IO;

namespace Intelbras.CRM2013.Application.Plugin.itbc_perfil
{
    public class ManagerPreEvent : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);
            var ServicePerfilServices = new Intelbras.CRM2013.Domain.Servicos.PerfilServices(context.OrganizationName, context.IsExecutingOffline);
            try
            {
                Entity entidade = (Entity)context.InputParameters["Target"];
                Domain.Model.Perfil perfil = entidade.Parse<Domain.Model.Perfil>(context.OrganizationName, context.IsExecutingOffline, service);


                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    case Domain.Enum.Plugin.MessageName.Create:
                
                        if(perfil.status == (int)Domain.Enum.Pefil.Status.Configurado)
                            ServicePerfilServices.ConsultaBeneficioCompromisso(perfil.ID.Value);
                        
                        break;
                   
                    case Domain.Enum.Plugin.MessageName.Update:

                        if (perfil.status == (int)Domain.Enum.Pefil.Status.Configurado)
                            ServicePerfilServices.ConsultaBeneficioCompromisso(perfil.ID.Value);
                        
                        break;
                }
            }
            catch (Exception ex)
            {
                trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), "Account", DateTime.Now));
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
    
}
