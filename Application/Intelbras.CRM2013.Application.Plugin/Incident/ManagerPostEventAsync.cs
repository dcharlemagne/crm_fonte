using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.Model;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Application.Plugin.incident
{
    public class ManagerPostEventAsync : IPlugin
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

                            Ocorrencia ocorrencia = ((Entity)context.PostEntityImages["imagem"]).Parse<Ocorrencia>(context.OrganizationName, context.IsExecutingOffline, service);
                            if (entidade.Attributes.Contains("itbc_anexo"))
                            {
                                new RepositoryService(context.OrganizationName, false, service).Anexo.Create(new Anotacao()
                                {
                                    Assunto = "Primeiro contato com o cliente",
                                    Texto = ocorrencia.Anexo,
                                    EntidadeRelacionada = new SDKore.DomainModel.Lookup(entidade.Id, "incident")
                                }
                                );
                            }
                        }

                        break;

                    case Domain.Enum.Plugin.MessageName.Update:

                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            var entidade = context.PostEntityImages["imagem"];                            
                        }

                        break;

                    case Domain.Enum.Plugin.MessageName.Delete:

                        if (context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity)
                        {
                            var entidade = context.PreEntityImages["imagem"];

                        }

                        break;

                    case Domain.Enum.Plugin.MessageName.SetStateDynamicEntity:

                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            var entidade = context.PostEntityImages["imagem"];

                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), "incident", DateTime.Now));
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
