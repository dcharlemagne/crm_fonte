using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Repository;
using Tridea.Framework.DomainModel;
using System.Web.Services.Protocols;
using Intelbras.Crm.Domain.Services;

namespace Intelbras.Crm.Application.Plugin.new_calendario_participante
{
    public class PreDelete : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                DynamicEntity preImage = (DynamicEntity)context.PreEntityImages["ParticipanteImage"];
                if (!preImage.Properties.Contains("new_participantesid")) return;

                Guid agendaId = new Guid(((Lookup)preImage.Properties["new_participantesid"]).Value.ToString());
                Agenda agenda = new Agenda(new Organizacao(context.OrganizationName)) { Id = agendaId };

                if (!agenda.PodeExcluirParticipante())
                    throw new InvalidPluginExecutionException("Este registro não pode ser excluido: A Agenda esta inativa");
            }
            catch (InvalidPluginExecutionException ex) { throw ex; }
            catch (Exception ex) { LogService.GravaLog(ex, TipoDeLog.PluginNew_calendario_participante); }
        }
    }
}
