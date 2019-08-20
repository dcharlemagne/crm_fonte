using System;

namespace Intelbras.CRM2013.Application.AtualizarValoresFaturamento
{
    class Program
    {
        private static string Organizacao
        {
            get { return SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"); }
        }

        private static bool IsOffline
        {
            get { return false; }
        }

        private static string PathTemp
        {
            get { return SDKore.Configuration.ConfigurationManager.GetSettingValue("DirMetaOrcamento"); }
        }

        static int Main(string[] args)
        {
            try
            {
                Console.WriteLine("{0} - Iniciando!", DateTime.Now);

                int ano, trimestre;

                if (args.Length == 0)
                {
                    args = new string[1];

                    Console.WriteLine("Informe a operação.");
                    args[0] = Console.ReadLine();
                }

                ano = (args.Length > 1) ? Convert.ToInt32(args[1]) : DateTime.Today.Year;
                trimestre = (args.Length > 2) ? Convert.ToInt32(args[2]) : new SDKore.Helper.DateTimeHelper().TrimestreAtual();
                var enumTrimestre = Domain.Servicos.Helper.ConverterTrimestreOrcamentoUnidade(trimestre);

                switch (args[0].ToUpper())
                {
                    case "ATUALIZAR_POTENCIAL_REPRESENTANTE":
                        new Domain.Servicos.PotencialdoKARepresentanteService(Organizacao, IsOffline)
                           .AtualizaValoresRealizado(ano, enumTrimestre);
                        break;

                    case "ATUALIZAR_POTENCIAL_SUPERVISOR":
                        new Domain.Servicos.PotencialdoSupervisorService(Organizacao, IsOffline)
                            .ImportarValores(ano, enumTrimestre, PathTemp);
                        break;
                            
                    case "ATUALIZAR_POTENCIAL_UNIDADE":
                        new Domain.Servicos.MetadaUnidadeService(Organizacao, IsOffline)
                              .ImportarValores(ano, enumTrimestre, PathTemp);
                        break;

                    case "ATUALIZAR_POTENCIAL_CANAL":
                        new Domain.Servicos.MetadoCanalService(Organizacao, IsOffline)
                              .AtualizarRealizado(ano, enumTrimestre);
                        break;
                }
                

                Console.WriteLine("{0} - Finalizando!", DateTime.Now);
                return 0;
            }
            catch (Exception ex)
            {
                string mensagem = SDKore.Helper.Error.Handler(ex);
                Console.WriteLine(mensagem);
                return ex.GetHashCode();
            }
        }
    }
}