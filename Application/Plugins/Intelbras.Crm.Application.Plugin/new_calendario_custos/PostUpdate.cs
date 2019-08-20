using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Services;

namespace Intelbras.Crm.Application.Plugin.new_calendario_custos
{
    public class PostUpdate : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                DynamicEntity preLead = (DynamicEntity)context.PreEntityImages["CustoImage"];
                if (!preLead.Properties.Contains("new_custosid")) return;

                Guid agendaId = ((Lookup)preLead.Properties["new_custosid"]).Value;

                Agenda agenda = new Agenda(new Organizacao(context.OrganizationName)) { Id = agendaId };
                agenda.Custo = agenda.ObterTotalDeCustos();
                agenda.Update();
            }
            catch (Exception ex) { LogService.GravaLog(ex, TipoDeLog.PluginNew_calendario_custos, "Intelbras.Crm.Application.Plugin.new_calendario_custos.PostUpdate"); }
        }
    }
}
