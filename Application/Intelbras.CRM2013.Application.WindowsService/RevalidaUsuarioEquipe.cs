using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using Intelbras.CRM2013.Domain;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.DAL;



namespace Intelbras.CRM2013.Application.WindowsService
{

    partial class RevalidaUsuarioEquipe : ServiceBase
    {



        public RevalidaUsuarioEquipe()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //this.timer = new System.Timers.Timer(60000D);  // 30000 milliseconds = 30 seconds
            //this.timer.AutoReset = true;
            //this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.timer_Elapsed);
            //this.timer.Start();        
        }
        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }

        public void RelavidarUsuariosEquipe()
        {
            List<Domain.Model.Conta> listContas = new Domain.Servicos.ContaService("INTELBRASQA", false).ListarTodasContas();

            foreach (Domain.Model.Conta registroConta in listContas)
            {
                
                //if (registroConta.Proprietario.Type == "team")
                //{
                //    //verifica todos os usuários que estão na equipes.
                //    List<Intelbras.CRM2013.Domain.Model.Equipe> membrosEquipe = new Intelbras.CRM2013.Domain.Servicos.EquipeService("INTELBRASQA", false).listarMembrosEquipe(registroConta.Proprietario.Id);

                //    foreach (Intelbras.CRM2013.Domain.Model.Equipe membroEquipe in membrosEquipe)
                //    {
                        
                //        //verifica se existe algum usuario na Equipe que não existe no registro
                //        Intelbras.CRM2013.Domain.Model.RelacionamentoCanal keyAccounts = new Intelbras.CRM2013.Domain.Servicos.RelacionamentoDoCanal("INTELBRASQA", false).ListarPorSupervisorOuAssistente((Guid)registroConta.ID, membroEquipe.ID.Value);

                //        //if (keyAccounts == null)
                //        //    new Domain.Servicos.RelacionamentoDoCanal("INTELBRASQA", false).RemoverUserEquipe(registroConta.ID.Value, membroEquipe.ID.Value);
                //    }
                //}
            }
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.RelavidarUsuariosEquipe(); // my separate static method for do work
        }

    }
}
