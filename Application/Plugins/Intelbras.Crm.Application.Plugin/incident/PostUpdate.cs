using System;
using System.Collections.Generic;
using Microsoft.Crm.Sdk;
using Tridea.Framework.DomainModel;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Model.Enum;
using Intelbras.Crm.Domain.Services;



/*
 * Plugin deve ser registrado.
 * Filtering Attributes: statuscode, statecode, new_reincidenteid, productid
 * Synchronous - Regra de negócio exige para que a Intervenção Técnica seja criada junto com a Alteração.
 */

namespace Intelbras.Crm.Application.Plugin.incident
{
    public class PostUpdate : IPlugin
    {
        Organizacao Organizacao = null;
        DynamicEntity EntidadeDoContexto = null;

        public void Execute(IPluginExecutionContext context)
        {
            DateTime inicioExecucao = DateTime.Now;
            try
            {
                var prop = context.InputParameters.Properties;

                if ((prop.Contains("Target") && prop["Target"] is DynamicEntity))
                {
                    this.EntidadeDoContexto = context.InputParameters.Properties["Target"] as DynamicEntity;
                }

                if (this.EntidadeDoContexto == null)
                {
                    return;
                }

                this.Organizacao = new Organizacao(context.OrganizationName);
                var id = PluginHelper.GetEntityId(context);

                var ocorrencia = DomainService.RepositoryOcorrencia.Retrieve(id);

                this.VerificaIntervencao(ocorrencia, context);
            }
            catch (Exception ex)
            {
                PluginHelper.TratarExcecao(ex, TipoDeLog.PluginIncident);
            }
        }


        private void VerificaIntervencao(Ocorrencia ocorrencia, IPluginExecutionContext context)
        {
            #region Validações

            if (ocorrencia == null)
            {
                return;
            }

            if (ocorrencia.DadosProduto == null)
            {
                return;
            }

            if (ocorrencia.StatusDaOcorrencia != StatusDaOcorrencia.Aguardando_Analise && ocorrencia.StatusDaOcorrencia != StatusDaOcorrencia.Aguardando_Peça)
            {
                return;
            }

            if (DomainService.RepositoryIntervencao.ListarPor(ocorrencia).Count > 0)
            {
                return;
            }

            #endregion


            LinhaComercial linhaComercial = DomainService.RepositoryLinhaComercialRepository.ObterPor(ocorrencia.DadosProduto, "new_dias_reincidencia", "new_numero_itens");


            if (linhaComercial != null && linhaComercial.NumeroDeDiasParaReincidencia > 0)
                if (context.SharedVariables.Contains("dataCriacaoReincidente"))
                {
                    DateTime dataCriacaoReincidente = Convert.ToDateTime(context.SharedVariables["dataCriacaoReincidente"]);
                    if (dataCriacaoReincidente.AddDays(linhaComercial.NumeroDeDiasParaReincidencia) >= ocorrencia.DataDeCriacao)
                    {
                        CriarIntervencaoTecnica(ocorrencia, "Intervenção Técnica por reincidente.");
                        return;
                    }
                }


            if (ocorrencia.DadosProduto.IntervencaoTecnica)
            {
                this.CriarIntervencaoTecnica(ocorrencia, "Produto da Ocorrência esta em Intervenção Técnica!");
                return;
            }

            List<Diagnostico> diagnosticos = DomainService.RepositoryDiagnostico.ListarPor(ocorrencia, "new_qtd_solicitada", "new_geratroca");
            int quantidade = 0;
            foreach (var item in diagnosticos)
                if ((bool)item.GeraTroca && item.QuantidadeSolicitada > 0)
                    quantidade += item.QuantidadeSolicitada;

            if (linhaComercial != null)
                if (linhaComercial.NumeroDeItensParaReincidencia > 0 && quantidade >= linhaComercial.NumeroDeItensParaReincidencia)
                {
                    CriarIntervencaoTecnica(ocorrencia, "Intervenção Técnica por quantidade de itens.");
                    return;
                }
        }

        private void CriarIntervencaoTecnica(Ocorrencia ocorrencia, string mensagem)
        {
            if (DomainService.RepositoryIntervencao.ObterPor(ocorrencia, "new_intervencao_tecnicaid") != null) return;

            IntervencaoTecnica intervencao = new IntervencaoTecnica(this.Organizacao)
            {
                OcorrenciaId = ocorrencia.Id,
                Nome = string.Format("{0} - {1}", ocorrencia.DadosProduto.CodigoEms, mensagem),
                StatusDaIntervencao = 1
            };

            DomainService.RepositoryIntervencao.Create(intervencao);
        }
    }
}