using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Model;
using System.Diagnostics;
using Tridea.Framework.DomainModel;
using Intelbras.Crm.Domain.Repository;
using Intelbras.Crm.Domain.Services;

namespace Intelbras.Crm.Application.Plugin.new_diagnostico_ocorrencia
{
    public class PostDelete : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            DateTime inicioExecucao = DateTime.Now;
            try
            {
                DynamicEntity preimage = (DynamicEntity)context.PreEntityImages["preimage"];

                //alterar status da ocorrencia
                if (preimage.Properties.Contains("new_ocorrenciaid"))
                {
                    PluginHelper.LogEmArquivo(context, "INICIO;", inicioExecucao.ToString(), "");
                    OcorrenciaService service = new OcorrenciaService(((Lookup)preimage["new_ocorrenciaid"]).Value);
                    service.AlterarStatusDaOcorrenciaParaOMenorStatusDosDiagnosticosRelacionados();
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
