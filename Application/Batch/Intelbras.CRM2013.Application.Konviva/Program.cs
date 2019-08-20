using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Servicos;
using System.IO;

namespace Intelbras.CRM2013.Application.Konviva
{
    class Program
    {
        private static string OrganizationName
        {
            get
            {
                return SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            }
        }

        static int Main(string[] args)
        {
            try
            {
                Console.WriteLine("Início Processamento de atualização dos Acessos Konviva.");
                Console.WriteLine(DateTime.Now.ToString());

                string opcao = "REVALIDARMAPEAMENTO";
                int quantidadeDias = 7;

                if (args != null && args.Length > 1)
                {
                    opcao = args[0];
                    quantidadeDias = int.Parse(args[1]);
                }

                string errorString = "";

                switch (opcao)
                {
                    case "REVALIDARMAPEAMENTO":
                        var dataInicial = DateTime.Today.AddDays(-quantidadeDias);
                        var conjuntoRespostaRequisicao = new DeParaDeUnidadeDoKonvivaService(OrganizationName, false).AtualizarAcessosKonvivaInconsistentes(dataInicial);

                        StringBuilder sb = new StringBuilder();

                        foreach (var conjuntoRespostaRequisicaoItem in conjuntoRespostaRequisicao)
                        {
                            if (conjuntoRespostaRequisicaoItem.Key != null && conjuntoRespostaRequisicaoItem.Value != null)
                            {
                                var conRespLocal = conjuntoRespostaRequisicaoItem;
                                if (conRespLocal.Key.IsFaulted)
                                {
                                    foreach (var item in conRespLocal.Key.Responses)
                                    {
                                        if (item.Fault != null)
                                        {
                                            sb.AppendLine(string.Format("Ocorreu uma falha enquanto {1} request era processada, no índice {0} da colecao da requisicao com a seguinte menssagem: {2}", item.RequestIndex + 1,
                                                    conRespLocal.Value.Requests[item.RequestIndex].RequestName,
                                                    item.Fault.Message));
                                        }
                                    }
                                }
                            }
                        }
                        
                        if (!sb.ToString().Equals(""))
                        {
                            if (sb.ToString().Length < 32000)
                                SDKore.Helper.Error.Create(sb.ToString(), System.Diagnostics.EventLogEntryType.Error);
                            else
                                using (StreamWriter w = File.AppendText("c:\\LogErrosKonviva" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + ".txt"))
                                {
                                    w.WriteLine(sb.ToString());
                                }
                        errorString = sb.ToString();
                        }
                            

                        break;
                }

                Console.WriteLine(errorString);
                Console.WriteLine(DateTime.Now.ToString());
                Console.WriteLine("Finalizando processo!");
                return 0;
        }
            catch (Exception ex)
            {
                string messageError = SDKore.Helper.Error.Handler(ex);
        Console.WriteLine("Erro : " + messageError);
                return ex.GetHashCode();
            }
}


    }
}
