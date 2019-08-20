using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelbras.CRM2013.Application.WindowsTask
{
    class WindowsTaskIntelbras
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Início Processamento Revalidação de usuários de equipe.");
                ValidacaoUsuario.RelavidarUsuariosEquipe();
                Console.WriteLine("Processamento de Revalidação de usuários de equipe foi finalizado com sucesso.");
                Console.WriteLine("");
                Console.WriteLine(" ----------------------- ");
                Console.WriteLine("");
                Console.WriteLine("Início Processamento Desativacao de Treinamentos Expirados.");
                ValidacaoTreinamento.DesativarTreinamentosExpirados();
                Console.WriteLine("Processamento de  Desativacao de Treinamentos Expirados foi finalizado com sucesso.");


            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro : " + ex.Message);
                throw new Exception(ex.Message);
            }
            
        }
    }
}
