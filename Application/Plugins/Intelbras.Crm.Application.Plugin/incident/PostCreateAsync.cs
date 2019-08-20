using System;
using Intelbras.Crm.Domain.Services;
using Microsoft.Crm.Sdk;

namespace Intelbras.Crm.Application.Plugin.incident
{
    public class PostCreateAsync : IPlugin
    {
        DynamicEntity EntidadeDoContexto = null;

        public void Execute(IPluginExecutionContext context)
        {
            DateTime inicioExecucao = DateTime.Now;
            try
            {
                if ((context.InputParameters.Properties.Contains("Target") && context.InputParameters.Properties["Target"] is DynamicEntity)) this.EntidadeDoContexto = context.InputParameters.Properties["Target"] as DynamicEntity;
                if (this.EntidadeDoContexto == null) return;

                PluginHelper.LogEmArquivo(context, "INICIO;", inicioExecucao.ToString(), "");
                FacadeOcorrencia facade = new FacadeOcorrencia(context);

                facade.PosAlteracao();

                PluginHelper.LogEmArquivo(context, "FIM;", inicioExecucao.ToString(), DateTime.Now.ToString());
            }
            catch (Exception ex) 
            {
                PluginHelper.TratarExcecao(ex, TipoDeLog.PluginIncident);
                PluginHelper.LogEmArquivo(context, "ERRO;", inicioExecucao.ToString(), DateTime.Now.ToString());
            }
        }

    }
}
