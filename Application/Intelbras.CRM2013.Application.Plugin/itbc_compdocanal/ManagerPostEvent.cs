using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Application.Plugin.itbc_compdocanal
{
    public class ManagerPostEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
            {
                case Domain.Enum.Plugin.MessageName.Create:
                    if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                    {
                        var entidade = (Entity)context.InputParameters["Target"];
                        Domain.Model.CompromissosDoCanal CompromissoTarget = entidade.Parse<Domain.Model.CompromissosDoCanal>(context.OrganizationName, context.IsExecutingOffline, adminService);
                        var compromissosDoCanalService = new CompromissosDoCanalService(context.OrganizationName, context.IsExecutingOffline, adminService);
                        CompromissosDoPrograma mCompromissosDoPrograma = compromissosDoCanalService.BuscarCompromissoDoPrograma(CompromissoTarget.Compromisso.Id);


                        #region Pendencias KeyAccount-representante
                        if (mCompromissosDoPrograma.TipoMonitoramento == (int)Domain.Enum.CompromissoPrograma.TipoMonitoramento.PorTarefas)
                        {
                            if (mCompromissosDoPrograma.Codigo.Value != (int)Domain.Enum.CompromissoPrograma.Codigo.Showroom)
                            {
                                List<string> lstAtividades = new TarefaService(context.OrganizationName, context.IsExecutingOffline, adminService).ListarAtividadesCheckup(CompromissoTarget.Compromisso.Id);

                                if (lstAtividades == null || lstAtividades.Count <= 0)
                                    throw new ArgumentException("(CRM) Lista de atividades não encontrada para o Compromisso : " + mCompromissosDoPrograma.Nome + " .Operação cancelada.");

                                string atividade = new TarefaService(context.OrganizationName, context.IsExecutingOffline, adminService).ObterProximaAtividadeCheckup(lstAtividades, null);

                                if (!string.IsNullOrEmpty(atividade))
                                {
                                    Domain.Model.Usuario proprietario = new UsuarioService(context.OrganizationName, context.IsExecutingOffline, adminService).BuscarProprietario("itbc_compdocanal", "itbc_compdocanalid", CompromissoTarget.Id);
                                    if (proprietario != null)
                                    {
                                        compromissosDoCanalService.GerarAtividadeChecklist(atividade, CompromissoTarget, proprietario);
                                    }
                                }
                            }
                        }
                        #endregion
                    }

                    break;
                case Domain.Enum.Plugin.MessageName.Update:
                    if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity &&
                        context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                    {
                        var entidade = (Entity)context.InputParameters["Target"];
                        Domain.Model.CompromissosDoCanal CompromissoPost = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.CompromissosDoCanal>(context.OrganizationName, context.IsExecutingOffline, adminService);
                        Domain.Model.CompromissosDoCanal CompromissoTarget = entidade.Parse<Domain.Model.CompromissosDoCanal>(context.OrganizationName, context.IsExecutingOffline, adminService);

                        if (CompromissoTarget.StatusCompromisso != null)
                        {
                            var compromissosDoCanalService = new CompromissosDoCanalService(context.OrganizationName, context.IsExecutingOffline, adminService);
                            // CASO DE USO 6 – ALTERAÇÃO DE STATUS DE COMPROMISSO DE UM CANAL - ATUALIZA STATUS DO BENEFÍCIO DO CANAL
                            compromissosDoCanalService.AtualizarBeneficiosECompromissosCascata(CompromissoPost);
                        }
                    }
                    break;
            }
        }
    }
}