using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using System.Xml.Linq;
using System.IO;

namespace Intelbras.CRM2013.Application.Plugin.itbc_denuncia
{
    public class ManagerPreEvent : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);
            try
            {
                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    case Domain.Enum.Plugin.MessageName.Update:
                        Entity entidade = (Entity)context.InputParameters["Target"];
                        Domain.Model.Denuncia denuncia = entidade.Parse<Domain.Model.Denuncia>(context.OrganizationName, context.IsExecutingOffline, service);

                        if (denuncia.RazaoStatus.HasValue && denuncia.RazaoStatus.Value == (int)Intelbras.CRM2013.Domain.Enum.Denuncia.StatusDenuncia.DenunciaProcedente)
                        {
                            Domain.Model.ParametroGlobal paramGlobal = new Domain.Model.ParametroGlobal(context.OrganizationName, context.IsExecutingOffline);

                            int tipoParametroGlobal = (int)Domain.Enum.TipoParametroGlobal.NumeroDiasCumprimentoCompromissosDenuncia;
                            paramGlobal = new Domain.Servicos.ParametroGlobalService(context.OrganizationName,
                                                context.IsExecutingOffline).ObterPor(tipoParametroGlobal, null, null, null, null, null, null,null);

                            if (paramGlobal != null)
                            {
                                entidade.Attributes["itbc_datacumprimento"] = DateTime.Now.AddDays(Convert.ToDouble(paramGlobal.Valor));
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), "itbc_denuncia", DateTime.Now));
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}

