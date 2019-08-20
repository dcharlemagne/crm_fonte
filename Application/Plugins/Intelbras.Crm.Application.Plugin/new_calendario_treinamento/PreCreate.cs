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
    public class PreCreate : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            DynamicEntity entidadeDoContexto = null;
            if (context.InputParameters.Properties.Contains("Target") && context.InputParameters.Properties["Target"] is DynamicEntity) entidadeDoContexto = (DynamicEntity)context.InputParameters.Properties["Target"];
            if (entidadeDoContexto == null) return;

            if (entidadeDoContexto.Properties.Contains("new_treinamentoid") && entidadeDoContexto.Properties.Contains("new_data_inicio"))
            {
                var treinamento = DomainService.RepositoryTreinamento.Retrieve(((Lookup)entidadeDoContexto.Properties["new_treinamentoid"]).Value);
                if (treinamento.ValidadeDoTreinamento < Convert.ToDateTime(((CrmDateTime)entidadeDoContexto.Properties["new_data_inicio"]).Value))
                    throw new InvalidPluginExecutionException("Operação não realizada. Não é possível criar nova Agenda quando a Data de Validade do Treinamento tiver expirado.");
            }

            if (entidadeDoContexto.Properties.Contains("new_clienteid"))
            {
                var cliente = DomainService.RepositoryCliente.Retrieve(((Lookup)entidadeDoContexto.Properties["new_clienteid"]).Value);
                if (cliente.Natureza == NaturezaDoCliente.PessoaFisica)
                    throw new InvalidPluginExecutionException("Operação não realizada. Não é possível criar nova Agenda para um Cliente Pessoa Física.");
            }

            if (!entidadeDoContexto.Properties.Contains("new_inscritos"))
                entidadeDoContexto.Properties.Add(new CrmNumberProperty("new_inscritos", new CrmNumber(0))); 
        }
    }
}