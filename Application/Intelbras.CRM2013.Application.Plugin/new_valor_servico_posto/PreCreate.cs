using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Model;
using System.Web.Services.Protocols;
using Intelbras.Crm.Domain.Services;

namespace Intelbras.Crm.Application.Plugin.new_valor_servico_posto
{
    public class PreCreate : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                DynamicEntity EntidadeDoContexto = null;
                if(context.InputParameters.Properties.Contains("Target") && context.InputParameters.Properties["Target"] is DynamicEntity)EntidadeDoContexto = context.InputParameters.Properties["Target"] as DynamicEntity;

                if (EntidadeDoContexto == null) return;

                if (!EntidadeDoContexto.Properties.Contains("new_linha_unidade_negocioid") && !EntidadeDoContexto.Properties.Contains("new_produtoid"))
                    throw new InvalidPluginExecutionException("Ação não executada. É necessário o preenchimentos de Produto ou Segmento.");
            }
            catch (InvalidPluginExecutionException ex) { throw ex; }
            catch (Exception ex)
            {
                LogService.GravaLog(ex, TipoDeLog.PluginNew_valor_servico_posto, "Intelbras.Crm.Application.Plugin.new_valor_servico_posto.PreCreate");
                throw ex;
            }

        }
    }
}