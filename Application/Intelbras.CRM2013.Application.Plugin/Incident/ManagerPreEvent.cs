using Microsoft.Xrm.Sdk;
using System;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.ViewModels;
using SDKore.Helper;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.Application.Plugin.Incident
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            try
            { 
                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    #region Create
                    case Domain.Enum.Plugin.MessageName.Create:
                        var parameterCreate = (Entity)context.InputParameters["Target"];
                        Ocorrencia ocorrenciaCreate = parameterCreate.Parse<Ocorrencia>(context.OrganizationName, context.IsExecutingOffline, adminService);

                        var ocorrenciaService = new OcorrenciaService(context.OrganizationName, context.IsExecutingOffline);
                        /*
                         * Caso a linha de contrato seja do tipo instalação e tenha valor no campo limite
                         * caso ultrapssse o limite definido da erro, e não deixa criar a ocorrencia
                         */
                        if( ocorrenciaCreate.LinhaDeContratoId != null )
                        {
                            RepositoryService repository = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline);
                            var linhaDeContrato = repository.LinhaDoContrato.Retrieve(ocorrenciaCreate.LinhaDeContratoId.Id);
                            if (linhaDeContrato != null && linhaDeContrato.TipoDeOcorrencia == (int)Domain.Enum.TipoDeOcorrencia.Instalacao && linhaDeContrato.LimiteOcorrencias > 0)
                            {
                                var listOcorrencias = ocorrenciaService.ListarOcorrenciasPorLinhaDoContrato(ocorrenciaCreate.LinhaDeContratoId.Id);
                                if(listOcorrencias.Count >= linhaDeContrato.LimiteOcorrencias)
                                {
                                    throw new ArgumentException("Erro ao criar Ocorrência. Limite de instalações atingido.");
                                }
                            }
                        }
                        ocorrenciaService.Ocorrencia = ocorrenciaCreate;
                        ocorrenciaService.Criar();

                        AtualizaCampos(ref parameterCreate, ocorrenciaService.Ocorrencia);

                        if (ocorrenciaCreate.Origem != null && (ocorrenciaCreate.Origem == (int) Domain.Enum.OrigemDaOcorrencia.PortalAssistenciaTencica || ocorrenciaCreate.Origem == (int)Domain.Enum.OrigemDaOcorrencia.OSIntegrada))
                        {
                            var usuario = new Guid(SDKore.Configuration.ConfigurationManager.GetSettingValue("guid_proprietario_portal_astec"));
                            parameterCreate.Attributes["ownerid"] = new EntityReference("systemuser", usuario);
                        }

                        break;
                    #endregion

                    #region Update
                    case Domain.Enum.Plugin.MessageName.Update:
                        var parameterUpdate = (Entity)context.InputParameters["Target"];
                        var entidadeComValoresFinais = (Entity)context.GetContextEntityMerge("imagem");
                        Ocorrencia ocorrenciaUpdate = entidadeComValoresFinais.Parse<Ocorrencia>(context.OrganizationName, context.IsExecutingOffline, adminService);
                        var ocorrenciaServiceUpdate = new OcorrenciaService(context.OrganizationName, context.IsExecutingOffline);

                        if( ocorrenciaUpdate.LinhaDeContratoId != null )
                        {
                            RepositoryService repository = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline);
                            var linhaDeContrato = repository.LinhaDoContrato.Retrieve(ocorrenciaUpdate.LinhaDeContratoId.Id);
                            if (linhaDeContrato != null && linhaDeContrato.TipoDeOcorrencia == (int)Domain.Enum.TipoDeOcorrencia.Instalacao && linhaDeContrato.LimiteOcorrencias > 0)
                            {
                                var listOcorrencias = ocorrenciaServiceUpdate.ListarOcorrenciasPorLinhaDoContrato(ocorrenciaUpdate.LinhaDeContratoId.Id);
                                if(listOcorrencias.Count >= linhaDeContrato.LimiteOcorrencias)
                                {
                                    var flag = true;

                                    foreach (var ocorrencia in listOcorrencias)
                                    {
                                        if(ocorrencia.Id.Equals(ocorrenciaUpdate.Id)){
                                            flag = false;
                                            break;
                                        }
                                    }

                                    if(flag)
                                        throw new ArgumentException("Erro ao atualizar Ocorrência. Limite de instalações atingido.");
                                    
                                }
                            }
                        }
                        
                        ocorrenciaServiceUpdate.Ocorrencia = ocorrenciaUpdate;
                        ocorrenciaServiceUpdate.Atualizar();
                        if (parameterUpdate.Attributes.Contains("statuscode"))
                            ocorrenciaServiceUpdate.AtualizarValorDoServicoASTEC();

                        AtualizaCampos(ref parameterUpdate, ocorrenciaServiceUpdate.Ocorrencia);

                        break;
                    #endregion

                    #region SetState
                    case Domain.Enum.Plugin.MessageName.SetStateDynamicEntity:

                        var state = (OptionSetValue)context.InputParameters["State"];
                        var status = (OptionSetValue)context.InputParameters["Status"];

                        var parameter = context.GetContextEntity("imagem");
                        Ocorrencia ocorrenciaSetState = parameter.Parse<Ocorrencia>(context.OrganizationName, context.IsExecutingOffline, adminService);

                        ocorrenciaSetState.Status = state.Value;
                        if (status.Value == 200040)
                        {
                            ocorrenciaSetState.StatusDaOcorrencia = Domain.Enum.StatusDaOcorrencia.Auditoria;
                            ocorrenciaSetState.RazaoStatus = status.Value;

                            var ocorrenciaServiceSetState = new OcorrenciaService(context.OrganizationName, context.IsExecutingOffline);
                            ocorrenciaServiceSetState.Ocorrencia = ocorrenciaSetState;

                            if (context.InputParameters.Contains("Status"))
                            {
                                ocorrenciaServiceSetState.AtualizarValorDoServicoASTEC();

                                new RepositoryService(context.OrganizationName, context.IsExecutingOffline, adminService).Ocorrencia.Update(ocorrenciaServiceSetState.Ocorrencia);
                            }
                        }
                  
                        break;
                     #endregion
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        private void AtualizaCampos(ref Entity parameter, Ocorrencia ocorrencia)
        {
            if (ocorrencia.DataSLA.HasValue)
            {
                parameter.Attributes.Remove("followupby");
                parameter.Attributes.Add("followupby", ocorrencia.DataSLA.Value);
            }
            if (ocorrencia.DataEscalacao.HasValue)
            {
                parameter.Attributes.Remove("new_data_hora_escalacao");
                parameter.Attributes.Add("new_data_hora_escalacao", ocorrencia.DataEscalacao.Value);
            }
            if (ocorrencia.OcorrenciaPaiId != null)
            {
                parameter.Attributes.Remove("new_reincidenteid");
                parameter.Attributes.Add("new_reincidenteid", new EntityReference("incident", ocorrencia.OcorrenciaPaiId.Id));
            }
            if (ocorrencia.AssuntoId != null)
            {
                parameter.Attributes.Remove("subjectid");
                parameter.Attributes.Add("subjectid", new EntityReference("subject", ocorrencia.AssuntoId.Id));
            }
            if (ocorrencia.ValorServico.HasValue)
            {
                parameter.Attributes.Remove("new_valor_servico");
                parameter.Attributes.Add("new_valor_servico", new Money(ocorrencia.ValorServico.Value));
            }
            if (ocorrencia.EstruturaDeAssunto != null)
            {
                foreach (Assunto item in ocorrencia.EstruturaDeAssunto)
                {
                    if (item.TipoDeAssunto != TipoDeAssunto.Vazio)
                    {
                        parameter.Attributes.Remove(item.CampoRelacionadoNaOcorrencia);
                        parameter.Attributes.Add(item.CampoRelacionadoNaOcorrencia, new EntityReference("subject", item.Id));
                    }
                }
            }
            //Salva o campo da Unidade de Negócio ASTEC
            if (ocorrencia.ProdutoId != null && ocorrencia.Produto != null)
            {
                if (ocorrencia.Produto.DadosFamiliaComercial != null
                    && ocorrencia.Produto.LinhaComercial != null
                    && ocorrencia.Produto.LinhaComercial.UnidadeDeNegocioId != null)
                {
                    parameter.Attributes.Remove("new_unidade_negocio_astec");
                    parameter.Attributes.Add("new_unidade_negocio_astec", new EntityReference("businessunit", ocorrencia.Produto.LinhaComercial.UnidadeDeNegocioId.Id));
                }
                    //ocorrencia.UnidadeDeNegocioAstecId = ocorrencia.Produto.DadosFamiliaComercial.LinhaComercial.UnidadeDeNegocioId;
            }
        }
    }
}