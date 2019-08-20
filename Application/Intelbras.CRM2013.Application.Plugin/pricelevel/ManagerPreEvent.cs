using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Application.Plugin.pricelevel
{
    public class ManagerPreEvent : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);
            var ServiceListaPMA = new Domain.Servicos.ListaPrecoService(context.OrganizationName, context.IsExecutingOffline, service);

            try
            {
                Domain.Model.ListaPreco targetListaPMA = ((Entity)context.InputParameters["Target"]).Parse<Domain.Model.ListaPreco>(context.OrganizationName, context.IsExecutingOffline);

                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    case Domain.Enum.Plugin.MessageName.Create:
                        if (ServiceListaPMA.ValidarExistencia(targetListaPMA, new List<Guid>()))
                            throw new ArgumentException("Registro de Lista PMA já existe com as mesmas características.");
                        break;
                    case Domain.Enum.Plugin.MessageName.Update:
                        Domain.Model.ListaPreco listaPMA = ((Entity)context.PreEntityImages["imagem"]).Parse<Domain.Model.ListaPreco>(context.OrganizationName, context.IsExecutingOffline);

                        if ((targetListaPMA.Status.HasValue && targetListaPMA.Status.Value == (int)Domain.Enum.ListaPreco.State.Ativo)
                            || (!targetListaPMA.Status.HasValue && listaPMA.Status.Value == (int)Domain.Enum.ListaPreco.State.Ativo))
                        {
                            //Caso mudou esses valores, altera na imagem
                            if (targetListaPMA.DataInicio.HasValue)
                                listaPMA.DataInicio = targetListaPMA.DataInicio;

                            if (targetListaPMA.DataTermino.HasValue)
                                listaPMA.DataTermino = targetListaPMA.DataTermino;

                            if (targetListaPMA.UnidadeNegocio != null)
                                listaPMA.UnidadeNegocio = targetListaPMA.UnidadeNegocio;

                            if (ServiceListaPMA.ValidarExistencia(listaPMA, new List<Guid>()))
                                throw new ArgumentException("Registro de Lista PMA já existe com as mesmas características.");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), "pricelevel", DateTime.Now));
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
