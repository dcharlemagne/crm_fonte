using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;
using System;

namespace Intelbras.CRM2013.Application.Plugin.itbc_pausa_de_tarefa
{
    public class ManagerPostEvent : IPlugin
    {
        private Object thisLock = new Object();

        public void Execute(IServiceProvider serviceProvider)
        {
            lock (thisLock)
            {
                var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(null);

                PausaTarefaService pausaTarefaService = new PausaTarefaService(context.OrganizationName, context.IsExecutingOffline, service);

                try
                {
                    trace.Trace(context.MessageName);

                    switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                    {
                        case Domain.Enum.Plugin.MessageName.Create:
                            var entidade = (Entity)context.InputParameters["Target"];

                            //altera a entidade Tarefa para ficar com o mesmo motivo da Pausa
                            var pausaCreate = pausaTarefaService.BuscaPausaTarefa(entidade.Id);
                            if (!pausaTarefaService.PersistirMotivoPausaNaTarefa(pausaCreate))
                                throw new ArgumentException("Não foi possível atualizar o Motivo da pausa na Tarefa.");

                            break;

                        case Domain.Enum.Plugin.MessageName.Update:

                            if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                            {
                                PausaTarefa pausaUpdate = context.PostEntityImages["imagem"].Parse<PausaTarefa>(context.OrganizationName, context.IsExecutingOffline, service);
                                if (pausaUpdate.DataTermino != null || pausaUpdate.DataTermino != DateTime.MinValue)
                                {
                                    //atribui vazio para o Motivo da Pausa na Tarefa
                                    var tarefa = pausaTarefaService.BuscaTarefa(pausaUpdate.Tarefa.Id);
                                    tarefa.MotivoPausa = " ";  //para salvar no banco, não funciona se usar null ou string.Empty

                                    TarefaService tarefaService = new TarefaService(context.OrganizationName, context.IsExecutingOffline, service);
                                    tarefaService.Persistir(tarefa);
                                }
                            }

                            break;
                    }
                }
                catch (Exception ex)
                {
                    string message = SDKore.Helper.Error.Handler(ex);

                    trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                    throw new InvalidPluginExecutionException(message, ex);
                }
            }
        }
    }
}
