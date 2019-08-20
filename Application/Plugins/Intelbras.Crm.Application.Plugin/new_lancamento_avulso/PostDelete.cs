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
using System.Web.Services.Protocols;

namespace Intelbras.Crm.Application.Plugin.new_lancamento_avulso
{
    public class PostDelete : IPlugin
    {
        Organizacao Organizacao = null;

        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                this.Organizacao = new Organizacao(context.OrganizationName);

                if (!context.PreEntityImages.Contains("Target")) return;
                DynamicEntity entity = (DynamicEntity)context.PreEntityImages["Target"];

                if (!entity.Properties.Contains("new_extratoid")) return;

                Guid extratoId = ((Lookup)entity.Properties["new_extratoid"]).Value;

                Extrato extrato = new Extrato(this.Organizacao) { Id = extratoId };
                extrato.AtualizarValor();
                DomainService.RepositoryExtrato.Update(extrato);
            }
            catch (Exception ex) { LogService.GravaLog(ex, TipoDeLog.PluginNew_lancamento_avulso, "PostDelete"); }
        }
    }
}
