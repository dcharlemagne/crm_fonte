using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.SdkTypeProxy;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Services;

namespace Intelbras.Crm.Application.Plugin.new_calendario_custos
{
    public class PostCreate : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                DynamicEntity entity = null;
                var isDynamicEntity = (context.InputParameters.Properties.Contains("Target") && context.InputParameters.Properties["Target"] is DynamicEntity);

                if (isDynamicEntity)
                {
                    entity = context.InputParameters.Properties["Target"] as DynamicEntity;
                    if (!entity.Properties.Contains("new_custosid")) return;

                    var organizacao = new Organizacao(context.OrganizationName);
                    Guid agendaId = ((Lookup)entity.Properties["new_custosid"]).Value;


                    Agenda agenda = new Agenda(new Organizacao(context.OrganizationName)) { Id = agendaId };
                    agenda.Custo = agenda.ObterTotalDeCustos();
                    agenda.Update();
                }
            }
            catch (Exception ex) { LogService.GravaLog(ex, TipoDeLog.PluginNew_calendario_custos, "Intelbras.Crm.Application.Plugin.new_calendario_custos.PostUpdate"); }
        }
    }
}
