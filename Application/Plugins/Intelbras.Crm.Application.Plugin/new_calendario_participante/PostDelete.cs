using System;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Services;
using Microsoft.Crm.Sdk;

namespace Intelbras.Crm.Application.Plugin.new_calendario_participante
{
    public class PostDelete : IPlugin
    {
        Organizacao organizacao;

        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                organizacao = new Organizacao(context.OrganizationName);

                DynamicEntity preImage = (DynamicEntity)context.PreEntityImages["ParticipanteImage"];
                if (!preImage.Properties.Contains("new_participantesid")) return;

                Guid agendaId = new Guid(((Lookup)preImage.Properties["new_participantesid"]).Value.ToString());

                int inscritos = DomainService.RepositoryAgenda.ObterQuantidadeInscritos(agendaId);

                Agenda agenda = new Agenda(organizacao) { Id = agendaId, Inscritos = (inscritos < 1) ? 0 : inscritos };
                DomainService.RepositoryAgenda.Update(agenda);
            }
            catch (Exception ex) { LogService.GravaLog(ex, TipoDeLog.PluginNew_calendario_participante); }
        }
    }
}
