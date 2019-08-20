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
    public class PreCreate : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            DateTime inicioExecucao = DateTime.Now;
            try
            {
                DynamicEntity entidade = null;
                if (context.InputParameters.Properties.Contains("Target") && context.InputParameters.Properties["Target"] is DynamicEntity)
                    entidade = (DynamicEntity)context.InputParameters.Properties["Target"];
                if (entidade == null) return;

                PluginHelper.LogEmArquivo(context, "INICIO;", inicioExecucao.ToString(), "");

                if (entidade.Properties.Contains("new_ocorrenciaid")
                 && entidade.Properties.Contains("new_produtoid")
                 && entidade.Properties.Contains("new_servicoid")
                 && entidade.Properties.Contains("new_defeitoid")
                 && entidade.Properties.Contains("new_nota_fiscal")
                 && entidade.Properties.Contains("new_serie_nota_fiscal"))
                {
                    if (DomainService.RepositoryDiagnostico.ObterDuplicidade(
                        ((Lookup)entidade.Properties["new_ocorrenciaid"]).Value,
                        ((Lookup)entidade.Properties["new_produtoid"]).Value,
                        ((Lookup)entidade.Properties["new_servicoid"]).Value,
                        ((Lookup)entidade.Properties["new_defeitoid"]).Value,
                        entidade.Properties["new_nota_fiscal"].ToString(),
                        entidade.Properties["new_serie_nota_fiscal"].ToString(),
                        Guid.Empty
                        ) != null)
                        throw new InvalidPluginExecutionException("Operação não realizada. Foi identificado diagnóstico redundante. Verifique 'Ocorrência', 'Produto', 'Serviço', 'Defeito', 'Nota Fiscal', 'Série NF'.");
                }


                entidade = ValidaStatusDoDiagnostico(entidade);
                PluginHelper.LogEmArquivo(context, "FIM;", inicioExecucao.ToString(), DateTime.Now.ToString());
             
            }
            catch (Exception ex)
            {
                PluginHelper.TratarExcecao(ex, TipoDeLog.PluginNew_diagnostico_ocorrencia);
                PluginHelper.LogEmArquivo(context, "ERRO;", inicioExecucao.ToString(), DateTime.Now.ToString());
            }
        }

        private DynamicEntity ValidaStatusDoDiagnostico(DynamicEntity entidade)
        {
            var quantidadeEhNulaOuZero = true;
            var geraTroca = true;

            if (entidade.Properties.Contains("new_qtd_solicitada"))
                if ((entidade.Properties["new_qtd_solicitada"] as CrmNumber).Value != 0)
                    quantidadeEhNulaOuZero = false;

            if (entidade.Properties.Contains("new_gera_troca"))
                if ((entidade.Properties["new_gera_troca"] as CrmBoolean).Value == false)
                    geraTroca = false;

            if (quantidadeEhNulaOuZero || !geraTroca)
            {
                entidade = PluginHelper.AdicionarPropriedadeEmEntidadeDinamica(entidade, "statuscode", new Status() { Value = 4 });
            }

            return entidade;
        }
    }
}
