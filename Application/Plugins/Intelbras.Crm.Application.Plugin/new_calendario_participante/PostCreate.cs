using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Model;
using Tridea.Framework.DomainModel;
using Intelbras.Crm.Domain.Repository;

namespace Intelbras.Crm.Application.Plugin.new_calendario_participante
{
    public class PostCreate : IPlugin
    {
        Organizacao organizacao;

        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                DynamicEntity entity = ValidaContexto(context);
                organizacao = new Organizacao(context.OrganizationName);                

                int inscritos = int.Parse(context.SharedVariables["inscritos"].ToString());
                Guid agendaId = ((Lookup)entity.Properties["new_participantesid"]).Value;

                AtualizaAgenda(agendaId, inscritos);
            }
            catch (Exception ex)
            {
                PluginHelper.TratarExcecao(ex, Domain.Services.TipoDeLog.PluginNew_calendario_participante);
            }
        }

        private void AtualizaAgenda(Guid agendaId, int totalInscritos)
        {
            if (totalInscritos < 0)
                totalInscritos = 0;

            Agenda agenda = new Agenda(organizacao)
            {
                Id = agendaId,
                Inscritos = ++totalInscritos
            };

            DomainService.RepositoryAgenda.Update(agenda);
        }

        private DynamicEntity ValidaContexto(IPluginExecutionContext context)
        {
            if (!context.InputParameters.Properties.Contains("Target"))
                throw new ArgumentException("Target nao existe");

            if (!(context.InputParameters.Properties["Target"] is DynamicEntity))
                throw new ArgumentException("Target nao é do tipo DynamicEntity");

            DynamicEntity entity = context.InputParameters.Properties["Target"] as DynamicEntity;

            if (!context.SharedVariables.Contains("inscritos"))
                throw new ArgumentNullException("inscritos", "Variavel compartilhada esta vazia!");
            
            if (!entity.Properties.Contains("new_participantesid"))
                throw new ArgumentNullException("new_participantesid");

            return entity;
        }

    }
}