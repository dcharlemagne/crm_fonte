using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services.Protocols;
using Microsoft.Crm.Sdk;
using Tridea.Framework.DomainModel;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Repository;
using Intelbras.Crm.Domain.Services;

namespace Intelbras.Crm.Application.Plugin.new_diagnostico_ocorrencia
{
    public class PreUpdate : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                DynamicEntity entidade = null;

                if (context.InputParameters.Properties.Contains("Target") && context.InputParameters.Properties["Target"] is DynamicEntity)
                    entidade = (DynamicEntity)context.InputParameters.Properties["Target"];

                if (entidade == null)
                    return;

                if (!context.PreEntityImages.Contains("preimage"))
                    return;

                if (entidade.Properties.Contains("new_ocorrenciaid")
                    || entidade.Properties.Contains("new_produtoid")
                    || entidade.Properties.Contains("new_servicoid")
                    || entidade.Properties.Contains("new_defeitoid")
                    || entidade.Properties.Contains("new_nota_fiscal")
                    || entidade.Properties.Contains("new_serie_nota_fiscal"))
                {

                    DynamicEntity preimage = (DynamicEntity)context.PreEntityImages["preimage"];
                    Diagnostico diagnostico = InstanciarDiagnostico(entidade, preimage);

                    if (ObtemDuplicidade(diagnostico))
                    {
                        throw new InvalidPluginExecutionException("Operação não realizada. Foi identificado diagnóstico redundante. Verifique 'Ocorrência', 'Produto', 'Serviço', 'Defeito', 'Nota Fiscal', 'Série NF'.");
                    }
                }
            }
            catch (Exception ex)
            {
                PluginHelper.TratarExcecao(ex, TipoDeLog.PluginNew_diagnostico_ocorrencia);
            }
        }

        private Diagnostico InstanciarDiagnostico(DynamicEntity entidade, DynamicEntity preimage)
        {
            Diagnostico diagnostico = new Diagnostico();

            diagnostico.Id = (entidade["new_diagnostico_ocorrenciaid"] as Key).Value;

            if (entidade.Properties.Contains("new_ocorrenciaid"))
                diagnostico.Ocorrencia = new Ocorrencia() { Id = ((Lookup)entidade.Properties["new_ocorrenciaid"]).Value };
            else if (preimage.Properties.Contains("new_ocorrenciaid"))
                diagnostico.Ocorrencia = new Ocorrencia() { Id = ((Lookup)preimage.Properties["new_ocorrenciaid"]).Value };

            if (entidade.Properties.Contains("new_produtoid"))
                diagnostico.DadosProduto = new Produto() { Id = ((Lookup)entidade.Properties["new_produtoid"]).Value };
            else if (preimage.Properties.Contains("new_produtoid"))
                diagnostico.DadosProduto = new Produto() { Id = ((Lookup)preimage.Properties["new_produtoid"]).Value };

            if (entidade.Properties.Contains("new_servicoid"))
                diagnostico.Solucao = new Solucao() { Id = ((Lookup)entidade.Properties["new_servicoid"]).Value };
            else if (preimage.Properties.Contains("new_servicoid"))
                diagnostico.Solucao = new Solucao() { Id = ((Lookup)preimage.Properties["new_servicoid"]).Value };

            if (entidade.Properties.Contains("new_defeitoid"))
                diagnostico.Defeito = new DefeitoOcorrenciaCliente() { Id = ((Lookup)entidade.Properties["new_defeitoid"]).Value };
            else if (preimage.Properties.Contains("new_defeitoid"))
                diagnostico.Defeito = new DefeitoOcorrenciaCliente() { Id = ((Lookup)preimage.Properties["new_defeitoid"]).Value };

            if (entidade.Properties.Contains("new_nota_fiscal"))
                diagnostico.NumeroNotaFiscal = entidade.Properties["new_nota_fiscal"].ToString();
            else if (preimage.Properties.Contains("new_nota_fiscal"))
                diagnostico.NumeroNotaFiscal = preimage.Properties["new_nota_fiscal"].ToString();

            if (entidade.Properties.Contains("new_serie_nota_fiscal"))
                diagnostico.SerieNotaFiscal = entidade.Properties["new_serie_nota_fiscal"].ToString();
            else if (preimage.Properties.Contains("new_serie_nota_fiscal"))
                diagnostico.SerieNotaFiscal = preimage.Properties["new_serie_nota_fiscal"].ToString();

            return diagnostico;
        }

        private bool ObtemDuplicidade(Diagnostico diagnostico)
        {
            if (diagnostico.Ocorrencia == null)
                throw new InvalidPluginExecutionException("Ocorrência é obrigatório!");

            if (diagnostico.DadosProduto == null)
                throw new InvalidPluginExecutionException("Produto é obrigatório!");

            if (diagnostico.Solucao == null)
                throw new InvalidPluginExecutionException("Serviço é obrigatório!");

            if (diagnostico.Defeito == null)
                throw new InvalidPluginExecutionException("Defeito é obrigatório!");

            return (DomainService.RepositoryDiagnostico.ObterDuplicidade(diagnostico.Ocorrencia.Id, diagnostico.DadosProduto.Id, diagnostico.Solucao.Id, diagnostico.Defeito.Id, diagnostico.NumeroNotaFiscal, diagnostico.SerieNotaFiscal, diagnostico.Id) != null);
        }
    }
}
