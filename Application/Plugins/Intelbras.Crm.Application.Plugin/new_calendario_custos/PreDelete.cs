using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Model;
using Tridea.Framework.DomainModel;
using Intelbras.Crm.Domain.Repository;

namespace Intelbras.Crm.Application.Plugin.new_calendario_custos
{
    public class PreDelete : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            Custo custo = new Custo(new Organizacao(context.OrganizationName)) { Id = PluginHelper.GetEntityId(context) };
            if (custo.Agenda != null)
                if (!custo.Agenda.VerificaAlteracaoPorStatus())
                    throw new InvalidPluginExecutionException("Este registro não pode ser excluido: A Agenda esta inativa!");
        }
    }
}
