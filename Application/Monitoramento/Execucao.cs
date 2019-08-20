using Intelbras.CRM2013.Domain.Servicos;
using System;

namespace Intelbras.CRM2013.Application.Monitoramento
{
    public class Execucao
    {
        private string OrganizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
        private Boolean IsOffline = false;

        public void ExecutaTodosMonitoramento()
        {
            Console.WriteLine("{0} - Iniciando Monitoramento Automatico", DateTime.Now);
            MonitoramentoAutomatico();


            Console.WriteLine("{0} - Iniciando Monitoramento por Tarefa", DateTime.Now);
            MonitoramentoPorTarefa();


            Console.WriteLine("{0} - Iniciando Monitoramento por Solicitação", DateTime.Now);
            MonitoramentoManual();


            Console.WriteLine("{0} - Iniciando Monitoramento por Solicitação", DateTime.Now);
            MonitoramentoPorSolicitacoes();


            Console.WriteLine("{0} - Iniciando Monitoramento por Treinamento", DateTime.Now);
            MonitoramentoPorTreinamento();
        }

        private void MonitoramentoAutomatico()
        {
            try
            {
                BeneficiosCompromissosService service = new BeneficiosCompromissosService(OrganizationName, IsOffline);

                if (service.HojeDiaExecutarMonitoramentoAutomatico())
                {
                    service.MonitoramntoAutomaticoParaApuracaoDeCompromissosEBaneficiosPorFilialEMatriz();
                }
            }
            catch (Exception ex)
            {
                string mensagem = SDKore.Helper.Error.Handler(ex);
                Console.WriteLine("Erro no Monitoramento Automatico: " + mensagem);
            }
        }

        private void MonitoramentoPorTarefa()
        {
            try
            {
                TarefaService service = new TarefaService(OrganizationName, IsOffline);
                service.MonitoramentoPorTarefas();
            }
            catch (Exception ex)
            {
                string mensagem = SDKore.Helper.Error.Handler(ex);
                Console.WriteLine("Erro no Monitoramento Automatico: " + mensagem);
            }
        }

        private void MonitoramentoPorSolicitacoes()
        {
            try
            {
                CompromissosDoCanalService service = new CompromissosDoCanalService(OrganizationName, IsOffline);
                service.MonitoramentoPorSolicitacoes();
            }
            catch (Exception ex)
            {
                string mensagem = SDKore.Helper.Error.Handler(ex);
                Console.WriteLine("Erro no Monitoramento Automatico: " + mensagem);
            }
        }

        private void MonitoramentoManual()
        {
            try
            {
                CompromissosDoCanalService service = new CompromissosDoCanalService(OrganizationName, IsOffline);
                service.MonitoramentoManual();
            }
            catch (Exception ex)
            {
                string mensagem = SDKore.Helper.Error.Handler(ex);
                Console.WriteLine("Erro no Monitoramento Automatico: " + mensagem);
            }
        }

        private void MonitoramentoPorTreinamento()
        {
            try
            {
                TreinamentoService service = new TreinamentoService(OrganizationName, IsOffline);
                service.GeracaoTreinamentoECertificacaoDoCanal();
                service.VerificacaoDoStatusTreinamentosECertificacaoCanal();
            }
            catch (Exception ex)
            {
                string mensagem = SDKore.Helper.Error.Handler(ex);
                Console.WriteLine("Erro no Monitoramento Automatico: " + mensagem);
            }
        }
    }
}