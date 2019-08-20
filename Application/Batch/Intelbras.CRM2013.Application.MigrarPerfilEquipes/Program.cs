using System;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Aplication.MigrarPerfilEquipes
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                //args = new string[] { "CRM2013H", "ATUALIZARCEPREVENDASSELLOUT" };

                if (args == null || args.Length < 2)
                {
                    args = new string[2];

                    Console.WriteLine("Informe a organização.");
                    args[0] = Console.ReadLine();

                    Console.WriteLine("Informe a operação.");
                    args[1] = Console.ReadLine();
                }

                string organizationName = args[0];
            
                switch (args[1].ToUpper())
                {
                    case "ATUALIZARREVENDASSELLOUT":

                        var classificacao = new Classificacao(organizationName, false) { ID = new Guid("85D1C6E2-6DED-E311-9407-00155D013D38") };
                        var subClassificacao = new Subclassificacoes(organizationName, false) { ID = new Guid("2E4D4BFD-FA68-E411-940B-00155D014212") };

                        new Domain.Servicos.ContaService(organizationName, false).AtualizarInformacoesRevendaSellOut(classificacao, subClassificacao);
                        break;

                    case "ATUALIZARCEPREVENDASSELLOUT":

                        var classificacao1 = new Classificacao(organizationName, false) { ID = new Guid("85D1C6E2-6DED-E311-9407-00155D013D38") };
                        var subClassificacao1 = new Subclassificacoes(organizationName, false) { ID = new Guid("2E4D4BFD-FA68-E411-940B-00155D014212") };

                        new Domain.Servicos.ContaService(organizationName, false).AtualizarCepRevendaSellOut(classificacao1, subClassificacao1);
                        break;

                    case "ATUALIZARSEFAZREVENDASSELLOUT":

                        var classificacao2 = new Classificacao(organizationName, false) { ID = new Guid("85D1C6E2-6DED-E311-9407-00155D013D38") };
                        var subClassificacao2 = new Subclassificacoes(organizationName, false) { ID = new Guid("2E4D4BFD-FA68-E411-940B-00155D014212") };

                        new Domain.Servicos.ContaService(organizationName, false).AtualizarIbgeRevendaSellOut(classificacao2, subClassificacao2);
                        break;
                }
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
