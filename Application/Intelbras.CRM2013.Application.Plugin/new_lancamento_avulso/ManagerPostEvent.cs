using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.Model;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Application.Plugin.new_lancamento_avulso
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
                            var lancamento = entidade.Parse<LancamentoAvulsoDoExtrato>(context.OrganizationName, context.IsExecutingOffline, service);
                            if (lancamento.ExtratoId != null)
                            {
                                Extrato extrato = (new CRM2013.Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline)).Extrato.Retrieve(lancamento.ExtratoId.Id);
                                extrato.AtualizarValor();
                                new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline).Extrato.Update(extrato);
                            }
                        }

                        break;

                    case Domain.Enum.Plugin.MessageName.Update:

                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            var entidade = context.PostEntityImages["imagem"];
                            var lancamento = entidade.Parse<LancamentoAvulsoDoExtrato>(context.OrganizationName, context.IsExecutingOffline, service);
                            if (lancamento.ExtratoId != null)
                            {
                                Extrato extrato = (new CRM2013.Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline)).Extrato.Retrieve(lancamento.ExtratoId.Id);
                                extrato.AtualizarValor();
                                new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline).Extrato.Update(extrato);
                            }
                        }

                        break;

                    case Domain.Enum.Plugin.MessageName.Delete:

                        if (context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity)
                        {
                            var entidade = context.PreEntityImages["imagem"];
                            var lancamento = entidade.Parse<LancamentoAvulsoDoExtrato>(context.OrganizationName, context.IsExecutingOffline, service);
                            if (lancamento.ExtratoId != null)
                            {
                                new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline).Ocorrencia.AtualizaValoresDosLancamentosAvulsosNo(lancamento.ExtratoId.Id);
                                Extrato extrato = (new CRM2013.Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline)).Extrato.Retrieve(lancamento.ExtratoId.Id);
                                extrato.AtualizarValor();
                                new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline).Extrato.Update(extrato);
                            }
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), "new_lancamento_avulso", DateTime.Now));
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
