using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Application.Workflow.Helper
{
    //Troca do Distribuidor Preferencial
    //Deverá ser adicionada uma rotina no monitoramento de benefícios e compromissos do PCI para que no início de cada trimestre sejam identificados 
    //todas as revendas que precisam ter o distribuidor preferencial alterado.

    public class DistribuidorPreferencial : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
                IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
                IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);

                RepositoryService RepositoryService = new RepositoryService(workflowContext.OrganizationName, workflowContext.IsExecutingOffline);
                HistoricoDistribuidor historico = RepositoryService.HistoricoDistribuidor.Retrieve(workflowContext.PrimaryEntityId);
                string datasExecucao = SDKore.Configuration.ConfigurationManager.GetSettingValue("Intelbras.CRM2013.Application.Workflow.DataExecucaohistoricoDistribuidor", true);
                string[] datas = datasExecucao.Split(';');
                bool controleExecucao = false;
                foreach (string data in datas)
                {
                    if (Convert.ToDateTime(data + "/" + DateTime.Now.Year.ToString()) == DateTime.Now.Date)
                    {
                        //if (DateTime.Now.Day == 1 && (DateTime.Now.Month == 1 || DateTime.Now.Month == 4 || DateTime.Now.Month == 7 || DateTime.Now.Month == 10))
                        //if (DateTime.Now.Day == 24 && (DateTime.Now.Month == 1 || DateTime.Now.Month == 3 || DateTime.Now.Month == 7 || DateTime.Now.Month == 10))
                        //{
                        //1- Identificar os registros ativos na entidade "Histórico de Ditribuidores" cuja data de início seja maior ou igual a data de início do trimestre
                        if (historico.DataInicio.HasValue && historico.DataInicio.Value.Date <= DateTime.Now.Date && historico.DataFim.HasValue && historico.DataFim.Value.Date >= DateTime.Now.Date)
                        {
                            RepositoryService.HistoricoDistribuidor.AlterarStatus(historico.ID.Value, 993520000); //Fluxo Concluído 993.520.000

                            //2- Nas revendas relacionadas à esses registros, alterar o valor do atributo "Distribuidor Preferencial" para registrar o novo distribuidor. 
                            Conta conta = RepositoryService.Conta.Retrieve(historico.Revenda.Id);
                            conta.ID = historico.Revenda.Id;
                            RepositoryService.Conta.Update(conta);

                            //Cada alteração deverá disparar uma mensagem "MSG0072 - REGISTRA_CONTA" para atualizar os sistemas envolvidos.
                            string nomeAbrevMatriEconom = String.Empty;
                            string nomeAbrevRet = String.Empty;
                            string codigoClienteRet = String.Empty;
                            var mensagem = new Domain.Integracao.MSG0072(workflowContext.OrganizationName, workflowContext.IsExecutingOffline);
                            mensagem.Enviar(conta, ref nomeAbrevRet, ref codigoClienteRet, ref nomeAbrevMatriEconom);
                            controleExecucao = true;

                        }
                        else if (historico.DataFim.HasValue && historico.DataFim.Value.Date < DateTime.Now.Date)
                        {
                            RepositoryService.HistoricoDistribuidor.AlterarStatus(historico.ID.Value, 993520000); //Fluxo Concluído 993.520.000
                        }
                        else
                        {
                            //Mantém o registro ativo
                            RepositoryService.HistoricoDistribuidor.AlterarStatus(historico.ID.Value, 0);
                        }
                    }
                }
                if (!controleExecucao)
                {
                    //Mantém o registro ativo 
                    RepositoryService.HistoricoDistribuidor.AlterarStatus(historico.ID.Value, 0);
                }
            }
            catch (Exception e)
            {
                SDKore.Helper.Error.Create(e, System.Diagnostics.EventLogEntryType.Information);
                throw new InvalidWorkflowException(e.Message + " :: " + e.StackTrace, e);
            }
        }
    }
}