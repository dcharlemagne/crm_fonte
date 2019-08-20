using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.SdkTypeProxy;
using System.Diagnostics;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Model.Ocorrencias;

namespace Intelbras.Crm.Application.Plugin.new_lacamento_avulso
{
    public class PostDelete : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            DynamicEntity preLead = (DynamicEntity)context.PreEntityImages["LancamentoAvulsoImage"];
            Lookup lkpLobPre = (Lookup)preLead.Properties["new_extratoid"];
            var extratoId = lkpLobPre.Value.ToString();

            if (!string.IsNullOrEmpty(extratoId))
            {
                try
                {
                    Ocorrencia ocorrencia = new Ocorrencia(new Organizacao(context.OrganizationName)); ;
                    ocorrencia.AtualizaValoresDosLancamentosAvulsosNo(new Guid(extratoId));
                }
                catch { }
            }
        }
    }
}
