using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services.Protocols;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Services;

namespace Intelbras.Crm.Application.Plugin.new_valor_servico_posto
{
    public class PreUpdate : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                DynamicEntity EntidadeDoContexto = null;
                if (context.InputParameters.Properties.Contains("Target") && context.InputParameters.Properties["Target"] is DynamicEntity) EntidadeDoContexto = context.InputParameters.Properties["Target"] as DynamicEntity;

                if (EntidadeDoContexto == null) return;

                if (EntidadeDoContexto.Properties.Contains("new_linha_unidade_negocioid") || EntidadeDoContexto.Properties.Contains("new_produtoid"))
                {
                    DynamicEntity preImage = null;
                    if (context.PreEntityImages.Contains("preImage")) preImage = (DynamicEntity)context.PreEntityImages["preImage"];
                    if (preImage == null) return;

                    if (EntidadeDoContexto.Properties.Contains("new_linha_unidade_negocioid") && EntidadeDoContexto.Properties.Contains("new_produtoid"))
                        if (((Lookup)EntidadeDoContexto.Properties["new_linha_unidade_negocioid"]) == null && ((Lookup)EntidadeDoContexto.Properties["new_linha_unidade_negocioid"]) == null)
                            throw new InvalidPluginExecutionException("Ação não executada. É necessário o preenchimentos de Produto ou Segmento.");

                    if (EntidadeDoContexto.Properties.Contains("new_linha_unidade_negocioid"))
                        if (((Lookup)EntidadeDoContexto.Properties["new_linha_unidade_negocioid"]) == null)
                            if (!preImage.Properties.Contains("new_produtoid"))
                                throw new InvalidPluginExecutionException("Ação não executada. É necessário o preenchimentos de Produto ou Segmento.");

                    if (EntidadeDoContexto.Properties.Contains("new_produtoid"))
                        if (((Lookup)EntidadeDoContexto.Properties["new_produtoid"]) == null)
                            if (!preImage.Properties.Contains("new_linha_unidade_negocioid"))
                                throw new InvalidPluginExecutionException("Ação não executada. É necessário o preenchimentos de Produto ou Segmento.");
                }
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