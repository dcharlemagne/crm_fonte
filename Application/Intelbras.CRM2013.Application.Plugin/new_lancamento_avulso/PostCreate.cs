using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Model;
using Tridea.Framework.DomainModel;
using System.Diagnostics;
using Intelbras.Crm.Domain.Repository;
using Intelbras.Crm.Domain.Services;
using System.Web.Services.Protocols;

namespace Intelbras.Crm.Application.Plugin.new_lancamento_avulso
{
    public class PostCreate : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                DynamicEntity entity = null;
                if (context.InputParameters.Properties.Contains("Target") && context.InputParameters.Properties["Target"] is DynamicEntity) entity = (DynamicEntity)context.InputParameters.Properties["Target"];
                if (entity == null) return;
                if (!entity.Properties.Contains("new_extratoid")) return;

                Guid extratoId = ((Lookup)entity.Properties["new_extratoid"]).Value;
                Extrato extrato = new Extrato(DomainService.Organizacao) { Id = extratoId };
                extrato.AtualizarValor();
                DomainService.RepositoryExtrato.Update(extrato);
            }
            catch (Exception ex) { LogService.GravaLog(ex, TipoDeLog.PluginNew_lancamento_avulso, "PostCreate"); }
        }
    }
}
