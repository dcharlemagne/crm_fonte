using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intelbras.CRM2013.Domain;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.DAL;

namespace Intelbras.CRM2013.Application.WindowsTask
{
    public class ValidacaoTreinamento
    {
        public static void DesativarTreinamentosExpirados()
        {
            //string Org = "CRM2013D";
            string Org = "INTELBRASQA";
            List<Intelbras.CRM2013.Domain.Model.ColaboradorTreinadoCertificado> listTreinamentos = new Intelbras.CRM2013.Domain.Servicos.TreinamentoService(Org, false).ListarExpirados();

            if (listTreinamentos.Count > 0)
            {
                Console.WriteLine("Quantidade de Treinamentos Expirados para processamento : " + listTreinamentos.Count.ToString());

                new Intelbras.CRM2013.Domain.Servicos.TreinamentoService(Org, false).Desativar(listTreinamentos);
            }
            else
                Console.WriteLine("Não há Treinamentos Expirados para processamento.");


            Console.WriteLine("Treinamentos Expirados desativados.");
            Console.WriteLine("");
            Console.WriteLine("Fim do Processo");
            

        }
    }
}
