using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Application.Plugin.itbc_solicitacaodecadastro
{
    public class ManagerPostEvent : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            //Não deve enviar a Solicitação

            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);

            try
            {
                //Inicia o processo de geração de tarefas para os Participantes do Processo configurados.
                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    case Domain.Enum.Plugin.MessageName.Create:

                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                        {
                            var entidade = (Entity)context.InputParameters["Target"];
                            Domain.Model.SolicitacaoCadastro SolCadastro = entidade.Parse<Domain.Model.SolicitacaoCadastro>(context.OrganizationName, context.IsExecutingOffline, service);

                            new Intelbras.CRM2013.Domain.Servicos.ProcessoDeSolicitacoesService(context.OrganizationName, context.IsExecutingOffline, service).CriacaoDeTarefasSolicitacaoDeCadastro(SolCadastro, context.UserId, 1);
                            //if (!SolCadastro.IntegrarNoPlugin)
                            //{
                            //    string xmlResposta = new Intelbras.CRM2013.Domain.Servicos.ProcessoDeSolicitacoes(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(SolCadastro);
                            //}
                        }

                        break;

                    //case Domain.Enum.Plugin.MessageName.Update:
                    //    if (context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity &&
                    //       context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                    //    {
                    //        var CategoriaPost = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.SolicitacaoCadastro>(context.OrganizationName, context.IsExecutingOffline, service);
                    //        if (!CategoriaPost.IntegrarNoPlugin)
                    //        {
                    //            string xmlResposta = new Intelbras.CRM2013.Domain.Servicos.ProcessoDeSolicitacoes(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(CategoriaPost);
                    //        }
                    //    }
                    //    break;
                }
            }
            catch (Exception ex)
            {
                trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), "itbc_solicitacaodecadastro", DateTime.Now));
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
