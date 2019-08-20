using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services.Protocols;
using Microsoft.Crm.Sdk;
using Tridea.Framework.DomainModel;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Model.Enum;
using Intelbras.Crm.Domain.Repository;
using Intelbras.Crm.Domain.Services;

namespace Intelbras.Crm.Application.Plugin.incident
{
    public class PreUpdate : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            DateTime inicioExecucao = DateTime.Now;
            try
            {
                var prop = context.InputParameters.Properties;
                if (prop.Contains("Target") && prop["Target"] is DynamicEntity)
                {
                    PluginHelper.LogEmArquivo(context, "INICIO;", inicioExecucao.ToString(), "");
                    var organizacao = new Organizacao(context.OrganizationName);
                    var entidade = prop["Target"] as DynamicEntity;

                    FacadeOcorrencia facade = new FacadeOcorrencia(context);
                    facade.Atender();

                    #region inserir resultado do atendimento do contexto

                    #region reincidencia

                    if (facade.Ocorrencia.OcorrenciaPai != null)
                    {
                        if (facade.Ocorrencia.OcorrenciaPai.Id != Guid.Empty)
                            entidade = PluginHelper.AdicionarPropriedadeEmEntidadeDinamica(entidade, "new_reincidenteid", new Lookup("incident", facade.Ocorrencia.OcorrenciaPai.Id));

                        if (facade.Ocorrencia.OcorrenciaPai.DataDeCriacao != DateTime.MinValue)
                            context.SharedVariables.Properties.Add(
                                new PropertyBagEntry("dataCriacaoReincidente", facade.Ocorrencia.OcorrenciaPai.DataDeCriacao.ToString())
                                );
                    }

                    #endregion

                    #region SLA

                    var sla = new CrmDateTime() { IsNull = true, IsNullSpecified = true };
                    if (facade.Ocorrencia.DataSLA.HasValue)
                        sla = new CrmDateTime(facade.Ocorrencia.DataSLA.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    entidade = PluginHelper.AdicionarPropriedadeEmEntidadeDinamica(entidade, "followupby", sla);

                    var escalacao = new CrmDateTime() { IsNull = true, IsNullSpecified = true };
                    if (facade.Ocorrencia.DataEscalacao.HasValue)
                        escalacao = new CrmDateTime(facade.Ocorrencia.DataEscalacao.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    entidade = PluginHelper.AdicionarPropriedadeEmEntidadeDinamica(entidade, "new_data_hora_escalacao", escalacao);

                    #endregion

                    #region  árvore de assunto

                    if (facade.Ocorrencia.EstruturaDeAssunto != null)
                    {
                        foreach (Assunto item in facade.Ocorrencia.EstruturaDeAssunto)
                        {
                            if (item.TipoDeAssunto != TipoDeAssunto.Vazio)
                            {
                                entidade = PluginHelper.AdicionarPropriedadeEmEntidadeDinamica(entidade, item.CampoRelacionadoNaOcorrencia, new Lookup("subject", item.Id));
                            }
                        }
                    }

                    #endregion

                    #region Valor do Serviço

                    if (facade.Ocorrencia.ValorServico.HasValue)
                    {
                        var ValorServicoCRM = new CrmMoney(facade.Ocorrencia.ValorServico.Value);
                        entidade = PluginHelper.AdicionarPropriedadeEmEntidadeDinamica(entidade, "new_valor_servico", ValorServicoCRM);
                    }

                    #endregion

                    #endregion
                    PluginHelper.LogEmArquivo(context, "FIM;", inicioExecucao.ToString(), DateTime.Now.ToString());
                }
            }
            catch (Exception ex)
            {
                PluginHelper.TratarExcecao(ex, TipoDeLog.PluginIncident);
                PluginHelper.LogEmArquivo(context, "ERRO;", inicioExecucao.ToString(), DateTime.Now.ToString());
            }
        }
    }
}