using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services.Protocols;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Services;

namespace Intelbras.Crm.Application.Plugin.new_calendario_participante
{
    public class PreCreate : IPlugin
    {
        Organizacao organizacao;

        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                bool isDynamicEntity = (context.InputParameters.Properties.Contains("Target") && context.InputParameters.Properties["Target"] is DynamicEntity);
                if (isDynamicEntity)
                {
                    this.organizacao = new Organizacao(context.OrganizationName);
                    DynamicEntity entity = context.InputParameters.Properties["Target"] as DynamicEntity;

                    if (!entity.Properties.Contains("new_participantesid"))
                        throw new ArgumentException("O campo Agenda é Obrigatório!");

                    Guid agendaId = ((Lookup)entity.Properties["new_participantesid"]).Value;
                    Agenda agenda = new Agenda(this.organizacao) { Id = agendaId };

                    if (agenda.PodeAdicionarParticipantes())
                        context.SharedVariables.Properties.Add(new PropertyBagEntry("inscritos", agenda.Inscritos));
                    else
                        throw new InvalidPluginExecutionException("Operação não realizada. Não é possível adicionar participantes à Agenda selecionada. Ela está finalizada ou já atingiu o número de vagas.");
                }
            }
            catch (Exception ex) { PluginHelper.TratarExcecao(ex, TipoDeLog.PluginNew_calendario_participante); }
        }

    }
}