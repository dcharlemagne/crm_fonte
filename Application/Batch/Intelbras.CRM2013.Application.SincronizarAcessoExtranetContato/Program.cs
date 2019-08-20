using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelbras.CRM2013.Application.SincronizarAcessoExtranetContato
{
    class Program
    {
        static void Main(string[] args)
        {
            string organizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            var AcessoService = new Intelbras.CRM2013.Domain.Servicos.AcessoExtranetContatoService(organizationName, false);
            List<Domain.Model.AcessoExtranetContato> lstAcessoExtranet = AcessoService.ListarTodos();

            int contador = 0;
            int contadorErro = 0;
            int numeroTotal = lstAcessoExtranet.Count;
            foreach (Domain.Model.AcessoExtranetContato acessoExtranet in lstAcessoExtranet)
            {
                try
                {
                    contador++;
                    AcessoService.IntegracaoBarramento(acessoExtranet);
                    Console.WriteLine("Integrado com Sucesso !Registro : "+contador.ToString() + "/"+ numeroTotal.ToString());
                }
                catch (Exception e)
                {
                    contadorErro++;
                    Console.WriteLine("Erro de integração : " + e.Message + "Registro : " + contador.ToString() + "/" + numeroTotal.ToString() + "Número de Erros : " + contadorErro.ToString());
                    
                    continue;
                }
            }
            Console.ReadLine();
        }
    }
}
