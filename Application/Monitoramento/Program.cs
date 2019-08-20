using Intelbras.CRM2013.Domain.Servicos;
using System;

namespace Intelbras.CRM2013.Application.Monitoramento
{
    static class Program
    {
        private static string OrganizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
        private static bool IsOffline = false;

        static int Main(string[] args)
        {
            try
            {
                Console.WriteLine("{0} - Iniciando monitoramento", DateTime.Now);

                if (args == null || args.Length == 0)
                {
                    throw new ArgumentException("(CRM) Nenhum parametro foi enviado para executar o monitoramento!");
                }

                switch (args[0].ToUpper())
                {
                    case "MONITORAMENTO":
                        Execucao service = new Execucao();
                        service.ExecutaTodosMonitoramento();
                        break;

                    case "CANCELAMENTO_SOLICITACAO_BENEFICIO":
                        SolicitacaoBeneficioService solicitacaoBeneficioService = new SolicitacaoBeneficioService(OrganizationName, IsOffline);
                        solicitacaoBeneficioService.ServicoDiarioSolicitacaoDeBeneficio();
                        break;
                }

                Console.WriteLine("{0} - Finalizando monitoramento", DateTime.Now);
                return 0;
            }
            catch (Exception ex)
            {
                string message = SDKore.Helper.Error.Handler(ex);
                Console.WriteLine("{0} - Finalizando monitoramento [Apresentou erro]", DateTime.Now);
                Console.WriteLine(message);
                //Environment.Exit(ex.GetHashCode());
                return ex.GetHashCode();
            }
        }
    }
}