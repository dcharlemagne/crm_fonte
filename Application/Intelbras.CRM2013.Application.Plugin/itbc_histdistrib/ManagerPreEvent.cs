using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Application.Plugin.itbc_histdistrib
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            var historicoDistribuidorService = new Domain.Servicos.HistoricoDistribuidorService(context.OrganizationName, context.IsExecutingOffline, adminService);

            switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
            {
                case Domain.Enum.Plugin.MessageName.Create:

                    var entityTargetCreate = context.GetContextEntity();
                    var targetCreate = entityTargetCreate.Parse<Domain.Model.HistoricoDistribuidor>(context.OrganizationName, context.IsExecutingOffline, adminService);
                    targetCreate.Status = (int)Domain.Enum.HistoricoDistribuidor.Statecode.Ativo;
                    historicoDistribuidorService.ValidaCamposObrigatorios(targetCreate);
                    historicoDistribuidorService.ValidaDuplicidade(targetCreate, true);

                    var revenda = new Domain.Servicos.ContaService(context.OrganizationName, context.IsExecutingOffline, userService).BuscaConta(targetCreate.Revenda.Id);
                    var distribuidor = new Domain.Servicos.ContaService(context.OrganizationName, context.IsExecutingOffline, userService).BuscaConta(targetCreate.Distribuidor.Id);

                    PreencheNome(ref entityTargetCreate, ref revenda, ref distribuidor);

                    break;

                case Domain.Enum.Plugin.MessageName.Update:

                    var entityTargetMerge = context.GetContextEntityMerge("imagem");
                    var entityTargetUpdate = context.GetContextEntity();
                    var targetUpdate = entityTargetMerge.Parse<Domain.Model.HistoricoDistribuidor>(context.OrganizationName, context.IsExecutingOffline, adminService);
                    var entityTarget = (Entity)context.InputParameters["Target"];
                    var contaUp = entityTarget.Parse<Domain.Model.HistoricoDistribuidor>(context.OrganizationName, context.IsExecutingOffline, adminService);

                    entityTargetUpdate.Attributes["itbc_datafim"] = contaUp.DataFim;

                    if (targetUpdate.Status.Value == (int)Domain.Enum.HistoricoDistribuidor.Statecode.Ativo)
                    {
                        historicoDistribuidorService.ValidaCamposObrigatorios(targetUpdate);
                        historicoDistribuidorService.ValidaDuplicidade(targetUpdate, false);
                    }

                    var revendaUpdate = new Domain.Servicos.ContaService(context.OrganizationName, context.IsExecutingOffline, userService).BuscaConta(targetUpdate.Revenda.Id);
                    var distribuidorUpdate = new Domain.Servicos.ContaService(context.OrganizationName, context.IsExecutingOffline, userService).BuscaConta(targetUpdate.Distribuidor.Id);

                    PreencheNome(ref entityTargetUpdate, ref revendaUpdate, ref distribuidorUpdate);
                    break;
            }
        }

        private void PreencheNome(ref Entity target, ref Domain.Model.Conta revenda, ref Domain.Model.Conta distribuidor)
        {
            if (target.Attributes.Contains("itbc_name"))
            {
                target.Attributes.Remove("itbc_name");
            }


            string nome = (revenda.NomeFantasia + " - " + distribuidor.NomeFantasia);
            nome = (nome.Length >= 100 ? nome.Substring(0, 99) : nome);
            target.Attributes.Add("itbc_name", nome);
        }
    }
}