using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;
using System;

namespace Intelbras.CRM2013.Application.Plugin.new_diagnostico_ocorrencia
{
    public class ManagerPreEvent : PluginBase
    {

        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            switch (EnumConverter<MessageName>(context.MessageName.ToLower()))
            {
                #region Create

                case MessageName.Create:

                    var e = context.GetContextEntity();
                    var entityTargetCreate = (Entity)context.InputParameters["Target"];
                    // Proprietário do Portal Astec
                    var usuario = new Guid(SDKore.Configuration.ConfigurationManager.GetSettingValue("guid_proprietario_portal_astec"));
                    string proprietario = "";
                    if (entityTargetCreate.Attributes.Contains("ownerid"))
                    {
                        proprietario = entityTargetCreate.Attributes["ownerid"].ToString();
                    }
                    var diagnosticoCreate = e.Parse<Diagnostico>(context.OrganizationName, context.IsExecutingOffline);

                    // Verificar se vem do Portal Astec
                    if (proprietario == "")
                    {
                        entityTargetCreate.Attributes["ownerid"] = new EntityReference("systemuser", usuario);

                        if (diagnosticoCreate.OcorrenciaId != null && diagnosticoCreate.ProdutoId != null && diagnosticoCreate.SolucaoId != null && diagnosticoCreate.DefeitoId != null &&
                            new RepositoryService(context.OrganizationName, context.IsExecutingOffline).Diagnostico.ObterDuplicidade(diagnosticoCreate.OcorrenciaId.Id, diagnosticoCreate.ProdutoId.Id,
                            diagnosticoCreate.SolucaoId.Id, diagnosticoCreate.DefeitoId.Id, diagnosticoCreate.NumeroNotaFiscal, diagnosticoCreate.SerieNotaFiscal, Guid.Empty) != null)
                            throw new InvalidPluginExecutionException("Operação não realizada. Foi identificado diagnóstico redundante. Verifique 'Ocorrência', 'Produto', 'Serviço', 'Defeito', 'Nota Fiscal', 'Série NF'.");
                    }

                    ValidaStatusDoDiagnostico(ref e);
                    break;

                #endregion

                #region Update

                case MessageName.Update:

                    var entityMerge = context.PreEntityImages["imagem"];
                    var entityTargetUpdate = (Entity)context.InputParameters["Target"];
                    var diagnosticoUpdate = entityTargetUpdate.Parse<Diagnostico>(context.OrganizationName, context.IsExecutingOffline);
                    Guid proprietarioPortalAstecUpdate = new Guid(SDKore.Configuration.ConfigurationManager.GetSettingValue("guid_proprietario_portal_astec")); // Proprietário do Portal Astec

                    Diagnostico diagnostico = new RepositoryService(context.OrganizationName, context.IsExecutingOffline).Diagnostico.Retrieve(diagnosticoUpdate.Id);

                    Usuario usuarioProp = new CRM2013.Domain.Servicos.RepositoryService().Usuario.BuscarProprietario("new_diagnostico_ocorrencia", "new_diagnostico_ocorrenciaid", diagnosticoUpdate.Id);

                    if (usuarioProp.Id == proprietarioPortalAstecUpdate)
                    {
                        if (diagnosticoUpdate.OcorrenciaId != null && diagnosticoUpdate.ProdutoId != null && diagnosticoUpdate.SolucaoId != null && diagnosticoUpdate.DefeitoId != null &&
                        new RepositoryService(context.OrganizationName, context.IsExecutingOffline).Diagnostico.ObterDuplicidade(diagnosticoUpdate.OcorrenciaId.Id, diagnosticoUpdate.ProdutoId.Id,
                        diagnosticoUpdate.SolucaoId.Id, diagnosticoUpdate.DefeitoId.Id, diagnosticoUpdate.NumeroNotaFiscal, diagnosticoUpdate.SerieNotaFiscal, diagnosticoUpdate.Id) != null)
                            throw new InvalidPluginExecutionException("Operação não realizada. Foi identificado diagnóstico redundante. Verifique 'Ocorrência', 'Produto', 'Serviço', 'Defeito', 'Nota Fiscal', 'Série NF'.");
                    }

                    Ocorrencia ocor = (new Domain.Servicos.RepositoryService()).Ocorrencia.Retrieve(diagnostico.OcorrenciaId.Id);
                    if ((diagnosticoUpdate.RazaoStatus == 4 || diagnostico.RazaoStatus == 4) && ocor.DataDeConsertoInformada != null)
                    {
                        entityTargetUpdate.Attributes["statuscode"] = new OptionSetValue(5); // Conserto Realizado
                    }
                    break;

                    #endregion
            }
        }

        private void ValidaStatusDoDiagnostico(ref Entity entidade)
        {
            var quantidadeEhNulaOuZero = true;
            var geraTroca = true;

            if (entidade.Attributes.Contains("new_qtd_solicitada") && Convert.ToDecimal(entidade.Attributes["new_qtd_solicitada"]) != 0)
                quantidadeEhNulaOuZero = false;

            if (entidade.Attributes.Contains("new_gera_troca") && !Convert.ToBoolean(entidade.Attributes["new_gera_troca"]))
                geraTroca = false;

            if (quantidadeEhNulaOuZero || !geraTroca)
            {
                //Retrieve na Ocorrência para verificar se o conserto da peça já está concluída, quando for item substituto
                var ocorrencia = (new Domain.Servicos.RepositoryService()).Ocorrencia.Retrieve(((EntityReference)entidade["new_ocorrenciaid"]).Id);

                if (ocorrencia.DataDeConsertoInformada == null)
                {
                    entidade.Attributes["statuscode"] = new OptionSetValue(4); // Aguardando Conserto
                }
                else
                {
                    entidade.Attributes["statuscode"] = new OptionSetValue(5); // Conserto Realizado
                }
            }
        }
    }
}
