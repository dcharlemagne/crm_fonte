using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.Application.Plugin.Annotation
{
    public class ManagerPostEvent : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);
            var repositoryService = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline);
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            try
            {
                Entity entidade = new Entity();
                Domain.Model.Anotacao anotacao = new Domain.Model.Anotacao(context.OrganizationName, context.IsExecutingOffline);

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {

                    switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                    {
                        case Domain.Enum.Plugin.MessageName.Create:
                            entidade = (Entity)context.InputParameters["Target"];
                            anotacao = entidade.Parse<Domain.Model.Anotacao>(context.OrganizationName, context.IsExecutingOffline);

                            try
                            {
                                Guid guidLead = new Guid(anotacao.EntidadeRelacionada.Id.ToString());
                                var ocorrencia = repositoryService.Ocorrencia.Retrieve(guidLead);
                                if (ocorrencia != null)
                                {
                                    if (ocorrencia.IntegraAstec == (int)IntegrarASTEC.Sim)
                                    {
                                        string lstResposta = new Domain.Servicos.OcorrenciaService(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(ocorrencia);
                                    }
                                }

                                var cliente = repositoryService.ClientePotencial.Retrieve(guidLead);
                                if (cliente != null)
                                {
                                    repositoryService.ClientePotencial.Update(cliente);
                                }
                                else
                                {
                                    var oportunidade = repositoryService.Oportunidade.Retrieve(guidLead);
                                    if (oportunidade != null)
                                    {
                                        repositoryService.Oportunidade.Update(oportunidade);
                                    }
                                }
                            }
                            catch (System.Exception e)
                            {                            
                            }

                            break;

                        case Domain.Enum.Plugin.MessageName.Update:

                            if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                            {
                                entidade = (Entity)context.PostEntityImages["imagem"];
                                anotacao = entidade.Parse<Domain.Model.Anotacao>(context.OrganizationName, context.IsExecutingOffline);

                                try
                                {
                                    Guid guidLead = new Guid(anotacao.EntidadeRelacionada.Id.ToString());
                                    var ocorrencia = repositoryService.Ocorrencia.Retrieve(guidLead);
                                    if (ocorrencia != null)
                                    {
                                        if (ocorrencia.IntegraAstec == (int)IntegrarASTEC.Sim)
                                        {
                                            string lstResposta = new Domain.Servicos.OcorrenciaService(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(ocorrencia);
                                        }
                                    }

                                    var cliente = repositoryService.ClientePotencial.Retrieve(guidLead);
                                    if (cliente != null)
                                    {
                                        repositoryService.ClientePotencial.Update(cliente);
                                    }
                                    else
                                    {
                                        var oportunidade = repositoryService.Oportunidade.Retrieve(guidLead);
                                        if (oportunidade != null)
                                        {
                                            repositoryService.Oportunidade.Update(oportunidade);
                                        }
                                    }
                                }
                                catch (System.Exception e)
                                {                             
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), @"Postagem", DateTime.Now));
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}