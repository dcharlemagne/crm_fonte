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
    public class ValidacaoUsuario
    {
        public static void RelavidarUsuariosEquipe()
        {
            //string Org = "CRM2013D";
           string Org = "INTELBRASQA";
            List<Intelbras.CRM2013.Domain.Model.Conta> listContas = new Intelbras.CRM2013.Domain.Servicos.ContaService(Org, false).ListarTodasContas();

            if (listContas.Count > 0)
                Console.WriteLine("Quantidade de Contas para processamento : " + listContas.Count.ToString());
            else
                Console.WriteLine("Não há conta para processamento.");

            foreach (Intelbras.CRM2013.Domain.Model.Conta registroConta in listContas)
            {
                //Console.WriteLine("Verificando conta Guid : " + registroConta.ID.Value.ToString());
                if (registroConta.Proprietario.Type == "team")
                {
                    //verifica todos os usuários que estão na equipes.
                    List<Intelbras.CRM2013.Domain.Model.TeamMembership> membrosEquipe = new Intelbras.CRM2013.Domain.Servicos.EquipeService(Org, false).listarMembrosEquipe(registroConta.Proprietario.Id);
                    foreach (Intelbras.CRM2013.Domain.Model.TeamMembership membroEquipe in membrosEquipe)
                    {
                        //verifica se existe algum usuario na Equipe que não existe no registro
                        Intelbras.CRM2013.Domain.Model.RelacionamentoCanal Assistente = new Intelbras.CRM2013.Domain.Servicos.RelacionamentoCanalService(Org, false).ListarPorAssistente((Guid)registroConta.ID, membroEquipe.Usuario);
                        Intelbras.CRM2013.Domain.Model.RelacionamentoCanal Supervisor = new Intelbras.CRM2013.Domain.Servicos.RelacionamentoCanalService(Org, false).ListarPorSupervisor((Guid)registroConta.ID, membroEquipe.Usuario);

                        if (Assistente == null && Supervisor == null)
                        {
                            if(membroEquipe.Usuario != registroConta.CriadoPor.Id)
                                new Intelbras.CRM2013.Domain.Servicos.RelacionamentoCanalService(Org, false).RemoverUserEquipe(registroConta.Proprietario.Id, membroEquipe.Usuario);
                            
                            Console.WriteLine("Removendo membro da equipe conta :" + registroConta.ID.Value.ToString());
                        }
                    }
                }
            }
        }

    }
}
