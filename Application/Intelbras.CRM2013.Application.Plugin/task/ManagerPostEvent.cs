using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Application.Plugin.task
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

                TarefaService ServiceTarefas = new TarefaService(context.OrganizationName, context.IsExecutingOffline, service);
                CompromissosDoCanalService ServiceCompromissosDoCanal = new CompromissosDoCanalService(context.OrganizationName, context.IsExecutingOffline, service);

                try
                {
                    trace.Trace(context.MessageName);

                    Intelbras.CRM2013.Domain.Model.Tarefa mTarefa = null;
                    switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                    {
                        case Domain.Enum.Plugin.MessageName.Create:
                            var entidade = (Entity)context.InputParameters["Target"];
                            mTarefa = ServiceTarefas.BuscaTarefa(entidade.Id);
                            

                            new Intelbras.CRM2013.Domain.Servicos.TarefaService(context.OrganizationName, context.IsExecutingOffline, service).CriarParecerParaSolicitacao(mTarefa);
                            Guid tipoAtividadeExecucao;

                            if (!Guid.TryParse(SDKore.Configuration.ConfigurationManager.GetSettingValue("TipoAtividadeExecucao"), out tipoAtividadeExecucao))
                                throw new ArgumentException("(CRM) Faltando parâmetro TipoAtividadeExecucao no SDKore");
                            
                            trace.Trace("Parâmetro do Config: TipoAtividadeExecucao '{0}'", tipoAtividadeExecucao);

                            if (mTarefa.ReferenteA != null && mTarefa.TipoDeAtividade != null && mTarefa.TipoDeAtividade.Id == tipoAtividadeExecucao)
                            {
                                trace.Trace("Tarefa do tipo Execução.");

                                SolicitacaoBeneficio solBenef = new Intelbras.CRM2013.Domain.Servicos.SolicitacaoBeneficioService(context.OrganizationName, context.IsExecutingOffline, service).ObterPor(mTarefa.ReferenteA.Id);

                                if (solBenef == null)
                                {
                                    throw new ArgumentException("(CRM) Solicitação não encontrada.");
                                }

                                if (solBenef.TipoSolicitacao != null && solBenef.AjusteSaldo.Value)
                                {
                                    trace.Trace("Solicitação do tipo Ajuste.");

                                    Tarefa _mTarefa = new Intelbras.CRM2013.Domain.Model.Tarefa(context.OrganizationName, context.IsExecutingOffline, service);
                                    _mTarefa.ID = context.PrimaryEntityId;
                                    _mTarefa.Resultado = (int)Domain.Enum.Tarefa.Resultado.PagamentoEfetuadoPedidoGerado;
                                    _mTarefa.State = 1;
                                    string retorno;

                                    TarefaService tarefaService = new Intelbras.CRM2013.Domain.Servicos.TarefaService(context.OrganizationName, context.IsExecutingOffline, service);
                                    tarefaService.Persistir(_mTarefa, out retorno);

                                    trace.Trace(tarefaService.Trace.StringTrace.ToString());
                                    tarefaService.Trace.Save();
                                }
                            }

                            mTarefa.TempoAtuacao = 0;
                            break;

                        case Domain.Enum.Plugin.MessageName.SetStateDynamicEntity:

                            if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                            {
                                Tarefa Tarefa = context.PostEntityImages["imagem"].Parse<Tarefa>(context.OrganizationName, context.IsExecutingOffline, service);

                                if (Tarefa.ReferenteA == null || Tarefa.State.Value != (int)Domain.Enum.Tarefa.StateCode.Fechada)
                                {
                                    break;
                                }

                                if (Tarefa.ReferenteA.Type.ToLower() == SDKore.Crm.Util.Utility.GetEntityName<SolicitacaoBeneficio>().ToLower())
                                {
                                    new ProcessoDeSolicitacoesService(context.OrganizationName, context.IsExecutingOffline, service)
                                        .ConcluirTarefaSolicitacaoBeneficio(Tarefa, context.UserId);
                                }
                                else if (Tarefa.ReferenteA.Type.ToLower() == SDKore.Crm.Util.Utility.GetEntityName<SolicitacaoCadastro>().ToLower())
                                {
                                    new ProcessoDeSolicitacoesService(context.OrganizationName, context.IsExecutingOffline, service)
                                        .ConcluirTarefaSolicitacaoDeCadastro(Tarefa, context.UserId);
                                }
                                else if (Tarefa.ReferenteA.Type.ToLower() == SDKore.Crm.Util.Utility.GetEntityName<CompromissosDoCanal>().ToLower())
                                {
                                    new ProcessoDeSolicitacoesService(context.OrganizationName, context.IsExecutingOffline, service)
                                        .ConcluirTarefaCompromissoCanal(Tarefa);
                                }
                                else if (Tarefa.ReferenteA.Type.ToLower() == SDKore.Crm.Util.Utility.GetEntityName<Conta>().ToLower())
                                {
                                    if (Tarefa.TipoDeAtividade.Name.Contains("Checklist"))
                                    {
                                        #region Pendencia Key-Account comentada

                                        Conta canal = new Intelbras.CRM2013.Domain.Servicos.ContaService(context.OrganizationName, context.IsExecutingOffline, service).BuscaConta(Tarefa.ReferenteA.Id);
                                        if (canal == null || canal.Classificacao == null)
                                            throw new ArgumentException("(CRM) Conta cadastrada no campo 'Referente a' não encontrada!");

                                        ParametroGlobal paramGlobal = new Intelbras.CRM2013.Domain.Servicos.ParametroGlobalService(context.OrganizationName, context.IsExecutingOffline, service).ObterPor((int)Domain.Enum.TipoParametroGlobal.FrequenciaChecklist, null, canal.Classificacao.Id, null, null, null, null, (int)Domain.Enum.ParametroGlobal.Parametrizar.VisitaComercial);
                                        ParametroGlobal paramGlobalListaAtividades = new Intelbras.CRM2013.Domain.Servicos.ParametroGlobalService(context.OrganizationName, context.IsExecutingOffline, service).ObterPor((int)Domain.Enum.TipoParametroGlobal.AtividadesChecklist, null, canal.Classificacao.Id, null, null, null, null, (int)Domain.Enum.ParametroGlobal.Parametrizar.VisitaComercial);

                                        List<String> lstAtividades = new Intelbras.CRM2013.Domain.Servicos.TarefaService(context.OrganizationName, context.IsExecutingOffline, service).ConverterParametroParaLista(paramGlobalListaAtividades.Valor);

                                        if (lstAtividades.Count > 0)
                                        {
                                            string atividade = ServiceTarefas.ObterProximaAtividadeCheckup(lstAtividades, Tarefa.Assunto);

                                            if (!string.IsNullOrEmpty(atividade))
                                            {
                                                Domain.Model.Tarefa novaTarefa = new Domain.Model.Tarefa(context.OrganizationName, context.IsExecutingOffline, service);

                                                Domain.Model.TipoDeAtividade tipoAtividade = new Domain.Servicos.TarefaService(context.OrganizationName, context.IsExecutingOffline, service).BuscarTipoTarefa("Checklist");
                                                if (tipoAtividade != null)
                                                {
                                                    novaTarefa.TipoDeAtividade = new SDKore.DomainModel.Lookup(tipoAtividade.ID.Value, tipoAtividade.Nome, "");
                                                }

                                                novaTarefa.Assunto = atividade;
                                                novaTarefa.Conclusao = DateTime.Now.AddDays(Convert.ToInt16(paramGlobal.Valor));
                                                novaTarefa.ReferenteA = new SDKore.DomainModel.Lookup(canal.ID.Value, "account");

                                                novaTarefa.ID = new Domain.Servicos.TarefaService(context.OrganizationName, context.IsExecutingOffline, service).Persistir(novaTarefa);
                                                if (novaTarefa.ID.HasValue)
                                                {
                                                    Usuario proprietario = new Domain.Servicos.UsuarioService(context.OrganizationName, context.IsExecutingOffline, service).BuscarProprietario("task", "activityid", Tarefa.Id);
                                                    if (proprietario != null)
                                                    {
                                                        new Domain.Servicos.UtilService(context.OrganizationName, context.IsExecutingOffline, service).MudarProprietarioRegistro("systemuser", proprietario.ID.Value, "task", novaTarefa.ID.Value);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Domain.Model.CompromissosDoCanal CompromissoCanal = ServiceCompromissosDoCanal.BuscarPorGuid(Tarefa.ReferenteA.Id);

                                            if (CompromissoCanal != null && CompromissoCanal.Compromisso != null)
                                            {
                                                List<string> listaAtividadesCheckup2 = ServiceTarefas.ListarAtividadesCheckup(CompromissoCanal.Compromisso.Id);

                                                if (listaAtividadesCheckup2.Count > 0)
                                                {
                                                    string atividade = ServiceTarefas.ObterProximaAtividadeCheckup(listaAtividadesCheckup2, Tarefa.Assunto);

                                                    if (!string.IsNullOrEmpty(atividade))
                                                    {
                                                        Domain.Model.Usuario proprietario = new Domain.Servicos.UsuarioService(context.OrganizationName, context.IsExecutingOffline, service).BuscarProprietario("itbc_compdocanal", "itbc_compdocanalid", CompromissoCanal.Id);
                                                        if (proprietario != null)
                                                        {
                                                            new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(context.OrganizationName, context.IsExecutingOffline, service).GerarAtividadeChecklist(atividade, CompromissoCanal, proprietario);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                    }
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
