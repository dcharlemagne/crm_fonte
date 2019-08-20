using System;
using System.Collections.Generic;
using Microsoft.Crm.Sdk;
using Tridea.Framework.DomainModel;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Model.Enum;
using Intelbras.Crm.Domain.Services;

/*
 * Plugin deve ser registrado.
 * Filtering Attributes:   new_geratroca, new_ocorrenciaid, new_produtoid, new_qtd_solicitada, statuscode.
 * Stage:                  Synchronous - Regra de negócio exige para que a Intervenção Técnica seja criada junto com a Alteração.
 * Description:            SyncPostUpdate of new_diagnostico_ocorrencia in Parent Pipeline
 * 
 * Registrar a Imagem 
 * Image Type:     PostImage
 * Entity Alias:   postimage
 * Parameters:     new_ocorrenciaid
 */


namespace Intelbras.Crm.Application.Plugin.new_diagnostico_ocorrencia
{
    public class PostUpdate : IPlugin
    {
        public string Organizacao { get; set; }

        public void Execute(IPluginExecutionContext context)
        {
            DateTime inicioExecucao = DateTime.Now;
            try
            {
                if ((context.InputParameters.Properties.Contains("Target") && context.InputParameters.Properties["Target"] is DynamicEntity))
                {
                    var entidade = context.InputParameters.Properties["Target"] as DynamicEntity;

                    if (entidade == null)
                    {
                        return;
                    }

                    this.Organizacao = context.OrganizationName;
                    DynamicEntity postimage = null;
                    
                    if (context.PostEntityImages.Contains("postimage"))
                        postimage = (DynamicEntity)context.PostEntityImages["postimage"];
                    
                    if (postimage == null) 
                        return;


                    //alterar status da ocorrencia
                    if (postimage.Properties.Contains("new_ocorrenciaid") && entidade.Properties.Contains("statuscode"))
                    {
                        OcorrenciaService service = new OcorrenciaService(((Lookup)postimage["new_ocorrenciaid"]).Value);
                        service.AlterarStatusDaOcorrenciaParaOMenorStatusDosDiagnosticosRelacionados();
                    }


                    if (entidade.Properties.Contains("new_produtoid") ||
                        entidade.Properties.Contains("new_geratroca") ||
                        entidade.Properties.Contains("new_qtd_solicitada"))
                    {
                        Guid diagnosticoId = PluginHelper.GetEntityId(context);
                        var diagnostico = DomainService.RepositoryDiagnostico.Retrieve(diagnosticoId);
                        this.VerificaIntervencao(diagnostico, context);
                    }
                }
            }
            catch (Exception ex)
            {
                PluginHelper.TratarExcecao(ex, TipoDeLog.PluginNew_diagnostico_ocorrencia);
            }
        }

        private void VerificaIntervencao(Diagnostico diagnostico, IPluginExecutionContext context)
        {
            if (diagnostico.Ocorrencia == null)
            {
                return;
            }

            if (diagnostico.Ocorrencia.DadosProduto == null)
            {
                return;
            }

            if (DomainService.RepositoryIntervencao.ListarPor(diagnostico.Ocorrencia).Count > 0)
            {
                return;
            }

            var statusOcorrencia = diagnostico.Ocorrencia.StatusDaOcorrencia;
            if (statusOcorrencia != StatusDaOcorrencia.Aguardando_Analise && statusOcorrencia != StatusDaOcorrencia.Aguardando_Peça)
            {
                return;
            }


            if ((bool)diagnostico.GeraTroca && diagnostico.QuantidadeSolicitada > 0
                && diagnostico.DadosProduto != null && diagnostico.DadosProduto.IntervencaoTecnica)
            {
                this.CriarIntervencaoTecnica(diagnostico, "Produto do diagnostico esta em Intervenção Técnica.");
                return;
            }


            LinhaComercial linhaComercial = DomainService.RepositoryLinhaComercialRepository.ObterPor(diagnostico.Ocorrencia.DadosProduto, "new_dias_reincidencia", "new_numero_itens");

            if (linhaComercial == null)
                return;

            if (linhaComercial.NumeroDeDiasParaReincidencia <= 0)
                return;

            DateTime dataCriacaoReincidente = DomainService.RepositoryOcorrencia.ObterDataDeCriacaoDoReincidentePorDiagnostico(diagnostico.Id);

            if (dataCriacaoReincidente != DateTime.MinValue)
                if (dataCriacaoReincidente.AddDays(diagnostico.Ocorrencia.DadosProduto.DadosFamiliaComercial.LinhaComercial.NumeroDeDiasParaReincidencia) >= diagnostico.Ocorrencia.DataDeCriacao)
                {
                    CriarIntervencaoTecnica(diagnostico, "Intervenção Técnica por reincidente.");
                    return;
                }

            List<Diagnostico> diagnosticos = DomainService.RepositoryDiagnostico.ListarPor(diagnostico.Ocorrencia, "new_qtd_solicitada", "new_geratroca");
            int quantidade = 0;
            foreach (var item in diagnosticos)
                if ((bool)item.GeraTroca && item.QuantidadeSolicitada > 0)
                    quantidade += item.QuantidadeSolicitada;

            if (quantidade >= linhaComercial.NumeroDeItensParaReincidencia)
            {
                CriarIntervencaoTecnica(diagnostico, "Intervenção Técnica por quantidade de itens.");
                return;
            }
        }

        private void CriarIntervencaoTecnica(Diagnostico diagnostico, string mensagem)
        {
            IntervencaoTecnica intervencao = new IntervencaoTecnica(diagnostico.Organizacao)
            {
                OcorrenciaId = diagnostico.Ocorrencia.Id,
                Nome = string.Format("{0} - {1}", diagnostico.DadosProduto.CodigoEms, mensagem),
                StatusDaIntervencao = 1
            };

            DomainService.RepositoryIntervencao.Create(intervencao);
        }
    }
}
