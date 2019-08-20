using System;
using System.Collections.Generic;
using Microsoft.Crm.Sdk;
using Tridea.Framework.DomainModel;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Model.Enum;
using Intelbras.Crm.Domain.Services;
using Intelbras.Helper.Log;

/*
 * Plugin deve ser registrado.
 * Filtering Attributes: statuscode, statecode, new_reincidenteid, productid
 * Synchronous - Regra de negócio exige para que a Intervenção Técnica seja criada junto com a Criação.
 */

namespace Intelbras.Crm.Application.Plugin.incident
{
    public class PostCreate : IPlugin
    {
        Organizacao Organizacao = null;
        DynamicEntity EntidadeDoContexto = null;

        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                if ((context.InputParameters.Properties.Contains("Target") && context.InputParameters.Properties["Target"] is DynamicEntity))
                {
                    this.EntidadeDoContexto = context.InputParameters.Properties["Target"] as DynamicEntity;
                }
                else
                {
                    return;
                }

                this.Organizacao = new Organizacao(context.OrganizationName);
                var ocorrencia = DomainService.RepositoryOcorrencia.RetrieveBasico(PluginHelper.GetEntityId(context)
                                    ,"statuscode"
                                    ,"incidentid"
                                    ,"createdon");

                this.VerificaIntervencao(ocorrencia, context);
            }
            catch (Exception ex)
            {
                LogHelper.Process(ex, ClassificacaoLog.PluginIncident);
            }
        }

        private void VerificaIntervencao(Ocorrencia ocorrencia, IPluginExecutionContext context)
        {
            #region Validações

            if (ocorrencia == null)
                return;

            if (ocorrencia.DadosProduto == null)
                return;

            if (ocorrencia.StatusDaOcorrencia != StatusDaOcorrencia.Aguardando_Analise && ocorrencia.StatusDaOcorrencia != StatusDaOcorrencia.Aguardando_Peça)
                return;

            if (DomainService.RepositoryIntervencao.ObterPor(ocorrencia, "new_intervencao_tecnicaid") != null) 
                return;

            #endregion

            if (ocorrencia.DadosProduto.IntervencaoTecnica)
            {
                this.CriarIntervencaoTecnica(ocorrencia, "Produto da Ocorrência esta em Intervenção Técnica!");
                return;
            }

            LinhaComercial linhaComercial = DomainService.RepositoryLinhaComercialRepository.ObterPor(ocorrencia.DadosProduto, "new_dias_reincidencia", "new_numero_itens");

            if (linhaComercial == null)
                return;

            if (linhaComercial.NumeroDeDiasParaReincidencia > 0)
                if (context.SharedVariables.Contains("dataCriacaoReincidente"))
                {
                    DateTime dataCriacaoReincidente = Convert.ToDateTime(context.SharedVariables["dataCriacaoReincidente"]);

                    if (dataCriacaoReincidente.AddDays(linhaComercial.NumeroDeDiasParaReincidencia) >= ocorrencia.DataDeCriacao)
                    {
                        CriarIntervencaoTecnica(ocorrencia, "Intervenção Técnica por reincidente!");
                        return;
                    }
                }
        }

        private void CriarIntervencaoTecnica(Ocorrencia ocorrencia, string mensagem)
        {
            IntervencaoTecnica intervencao = new IntervencaoTecnica(this.Organizacao)
            {
                OcorrenciaId = ocorrencia.Id,
                Nome = string.Format("{0} - {1}", ocorrencia.DadosProduto.CodigoEms, mensagem),
                StatusDaIntervencao = (int)IntervencaoTecnicaEnum.StatusCode.AguardandoAnalise
            };

            DomainService.RepositoryIntervencao.Create(intervencao);
        }
    }
}