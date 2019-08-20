using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Services;

namespace Intelbras.Crm.Application.Plugin.new_diagnostico_ocorrencia
{
    public class PostSetStateDynamicEntity : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            DateTime inicioExecucao = DateTime.Now;
            try
            {
                if (context.PostEntityImages.Contains("postimage"))
                {
                    var postimage = (DynamicEntity)context.PostEntityImages["postimage"];
                    if (postimage == null) return;

                    PluginHelper.LogEmArquivo(context, "INICIO;", inicioExecucao.ToString(), "");

                    //alterar status da ocorrencia
                    if (postimage.Properties.Contains("new_ocorrenciaid"))
                    {
                        OcorrenciaService service = new OcorrenciaService(((Lookup)postimage["new_ocorrenciaid"]).Value);
                        service.AlterarStatusDaOcorrenciaParaOMenorStatusDosDiagnosticosRelacionados();
                    }
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
