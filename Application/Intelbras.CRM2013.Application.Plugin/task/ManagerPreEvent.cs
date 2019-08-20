using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;
using System;
using Intelbras.CRM2013.Domain.Servicos.GestaoSLA;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.Application.Plugin.task
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            switch (EnumConverter<MessageName>(context.MessageName.ToLower()))
            {
                #region Create

                case MessageName.Create:
                    break;
                #endregion

                #region Update
                case MessageName.Update:
                    break;

                #endregion

                #region SetStateDynamicEntity

                case MessageName.SetStateDynamicEntity:
                    EntityReference entity = (EntityReference)context.InputParameters["EntityMoniker"];
                    OptionSetValue state = (OptionSetValue)context.InputParameters["State"];
                    //OptionSetValue status = (OptionSetValue)context.InputParameters["Status"];
                    IOrganizationService service = serviceFactory.CreateOrganizationService(null);

                    if (state.Value == (int)Tarefa.StateCode.Fechada)
                    {
                        TarefaService ServiceTarefas = new TarefaService(context.OrganizationName, context.IsExecutingOffline, service);
                        var task = ServiceTarefas.BuscaTarefa(entity.Id);

                        //Alteração para obter tempo de resposta ignorando finais de semana e feriados
                        task.TempoResposta = (Decimal)CalcularDiferencaHorasEntreDatas(context, task.CriadoEm.Value.ToLocalTime(), DateTime.Now.ToLocalTime());

                        //calcula as pausas da Tarefa
                        PausaTarefaService pausaTarefaService = new PausaTarefaService(context.OrganizationName, context.IsExecutingOffline, service);


                        var pausas = pausaTarefaService.ListarPausaTarefa(entity.Id);
                        decimal TempoPausa = 0;

                        pausas.ForEach(p =>
                        {
                            //finaliza as pausas pendentes
                            if (p.DataTermino == null || p.DataTermino == DateTime.MinValue)
                            {
                                p.DataTermino = DateTime.Now.AddHours(3);
                                pausaTarefaService.Persistir(p);
                            }
                            //calcula o tempo das pausas (em minutos)
                            TempoPausa = (Decimal)CalcularDiferencaHorasEntreDatas(context, p.DataInicio.ToLocalTime(), p.DataTermino.ToLocalTime());
                        });

                        //atualiza o campo de tempo de atuação da tarefa
                        if (task.TempoResposta.HasValue)
                            task.TempoAtuacao = (task.TempoResposta.Value - TempoPausa);
                        else
                            task.TempoAtuacao = TempoPausa;

                        task.MotivoPausa = " "; //para salvar no banco, não funciona se usar null ou string.Empty

                        ServiceTarefas.Persistir(task);
                    }

                    break;

                    #endregion
            }
        }

        public double CalcularDiferencaHorasEntreDatas(IPluginExecutionContext context, DateTime dataInicial, DateTime dataFinal)
        {
            var diasDescontar = 0;
            var horasDescontar = 0;

            var parametroGlobalService = new ParametroGlobalService(context.OrganizationName, false);

            var parametroGlobalFeriados = parametroGlobalService.ObterPor((int)TipoParametroGlobal.Feriados);

            var datasFeriados = parametroGlobalFeriados.Valor;

            string[] feriadosFixos = new string[0];
            if (!string.IsNullOrEmpty(datasFeriados))
            {
                feriadosFixos = datasFeriados.Split(';');
            }

            CalendarioDeFeriados calendario = new CalendarioDeFeriados(context.OrganizationName, false);
            var outrosFeriados = calendario.Feriados;

            TimeSpan diferencaEmHoras = dataFinal - dataInicial;

            for (int i = 1; i <= diferencaEmHoras.TotalDays; i++)
            {
                dataInicial = dataInicial.AddDays(1);

                if (dataInicial.DayOfWeek == DayOfWeek.Sunday || dataInicial.DayOfWeek == DayOfWeek.Saturday)
                    diasDescontar++;

                foreach (string feriado in feriadosFixos)
                {
                    if (!string.IsNullOrEmpty(feriado) && !string.IsNullOrWhiteSpace(feriado)){
                        if (dataInicial.Date == Convert.ToDateTime(String.Format("{0}/{1}", feriado, DateTime.Now.Year.ToString())) &&
                            dataInicial.DayOfWeek != DayOfWeek.Sunday &&
                            dataInicial.DayOfWeek != DayOfWeek.Saturday)
                        {

                            diasDescontar++;
                        }
                    }
                }

                foreach (Feriado feriado in outrosFeriados)
                {
                    if (dataInicial.Date == feriado.DataInicio.Date &&
                        dataInicial.DayOfWeek != DayOfWeek.Sunday &&
                        dataInicial.DayOfWeek != DayOfWeek.Saturday)
                    {

                        diasDescontar++;
                    }
                }
            }

            horasDescontar = diasDescontar * 24;

            return diferencaEmHoras.TotalHours - horasDescontar;
        }
    }
}
