using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Tridea.Framework.DomainModel;
using Intelbras.Crm.Domain.Repository;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Model.Enum;

namespace Intelbras.Crm.Application.Plugin.new_calendario_treinamento
{
    public class PostUpdateAsync : IPlugin
    {
        DynamicEntity entidadeDoContexto = null;

        public void Execute(IPluginExecutionContext context)
        {

            if (context.InputParameters.Properties.Contains("Target") && context.InputParameters.Properties["Target"] is DynamicEntity) entidadeDoContexto = (DynamicEntity)context.InputParameters.Properties["Target"];
            if (entidadeDoContexto == null) return;

            if (entidadeDoContexto.Properties.Contains("statuscode") && ((Status)entidadeDoContexto.Properties["statuscode"]).Value == 3)
            {
                AtualizaFrequencia();
            }
        }

        private void AtualizaFrequencia()
        {
            var agenda = DomainService.RepositoryAgenda.Retrieve(((Key)entidadeDoContexto.Properties["new_calendario_treinamentoid"]).Value);

            var participantes = DomainService.RepositoryParticipanteTreinamento.ListarParticipantesPor(agenda);

            foreach (ParticipanteTreinamento participante in participantes)
            {
                if (String.IsNullOrEmpty(participante.Frequencia))
                {
                    participante.Frequencia = "100%";
                    DomainService.RepositoryParticipanteTreinamento.Update(participante);
                }
            }
        }
    }
}
