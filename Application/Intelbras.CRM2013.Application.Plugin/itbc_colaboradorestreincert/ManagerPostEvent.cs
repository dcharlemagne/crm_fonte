using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Application.Plugin.itbc_colaboradorestreincert
{
    public class ManagerPostEvent : IPlugin
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
                    case Domain.Enum.Plugin.MessageName.Create:

                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                        {
                            var entidade = (Entity)context.InputParameters["Target"];
                            Domain.Model.ColaboradorTreinadoCertificado colaboradorTreiCert = entidade.Parse<Domain.Model.ColaboradorTreinadoCertificado>(context.OrganizationName, context.IsExecutingOffline, service);

                            //if(colaboradorTreiCert.Canal != null)
                            //    new Domain.Servicos.TreinamentoService(context.OrganizationName, context.IsExecutingOffline, service).VerificaSeContatoEstaNoCanal(colaboradorTreiCert.Canal.Id, colaboradorTreiCert.Contato.Id);
                            
                            //new Domain.Servicos.TreinamentoService(context.OrganizationName, context.IsExecutingOffline, service).VerificaCumprimento(colaboradorTreiCert);
                        }

                        break;

                    case Domain.Enum.Plugin.MessageName.Update:
                        if (context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity &&
                            context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {

                            var colabPre = ((Entity)context.PreEntityImages["imagem"]).Parse<Domain.Model.ColaboradorTreinadoCertificado>(context.OrganizationName, context.IsExecutingOffline, service);
                            var colabPost = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.ColaboradorTreinadoCertificado>(context.OrganizationName, context.IsExecutingOffline, service);

                            if (colabPost.Canal != null)
                                new Domain.Servicos.TreinamentoService(context.OrganizationName, context.IsExecutingOffline, service).VerificaSeContatoEstaNoCanal(colabPost.Canal.Id, colabPost.Contato.Id, (Guid) colabPost.ID);

                            // COMENTADO POIS ROTINA ESTA CONTEMPLADA EM MONITORAMENTO DIARIO
                            //if (colabPre.Canal == null && colabPost.Canal != null)
                            //{
                            //    new Domain.Servicos.TreinamentoService(context.OrganizationName, context.IsExecutingOffline, service).VerificaCumprimento(colabPost);
                            //    return;
                            //}

                            //if (colabPre.Canal != null && colabPost.Canal == null)
                            //{
                            //    new Domain.Servicos.TreinamentoService(context.OrganizationName, context.IsExecutingOffline, service).VerificaCumprimento(colabPre);
                            //    return;
                            //}

                            if (colabPre.Canal != null && colabPost.Canal != null && colabPre.Canal.Id == colabPost.Canal.Id)
                                return;
                            
                            //new Domain.Servicos.TreinamentoService(context.OrganizationName, context.IsExecutingOffline, service).VerificaCumprimento(colabPre);
                            //new Domain.Servicos.TreinamentoService(context.OrganizationName, context.IsExecutingOffline, service).VerificaCumprimento(colabPost);

                        }
                        break;

                    case Domain.Enum.Plugin.MessageName.SetStateDynamicEntity:

                        if (context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity &&
                           context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            var colabPost = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.ColaboradorTreinadoCertificado>(context.OrganizationName, context.IsExecutingOffline, service);

                            new Domain.Servicos.TreinamentoService(context.OrganizationName, context.IsExecutingOffline, service).VerificaCumprimento(colabPost);
                        }

                    break;

                }
            }
            catch (Exception ex)
            {
                trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), "itbc_colaboradorestreincert", DateTime.Now));
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}