using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services.Protocols;
using Microsoft.Crm.Sdk;
using Tridea.Framework.DomainModel;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Repository;
using Intelbras.Crm.Domain.Services;

namespace Intelbras.Crm.Application.Plugin.new_extrato_logistica_reversa
{
    public class PreDelete : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                Extrato extrato = new Extrato() { Id = PluginHelper.GetEntityId(context) };
                DomainService.RepositoryDiagnostico.LimparCampoExtratoLogisticaReversa(extrato);
            }
            catch (Exception ex)
            {
                LogService.GravaLog(ex, TipoDeLog.PluginNew_extrato_logistica_reversa);
                throw new InvalidPluginExecutionException("Houve um problema ao tentar executar essa ação, tente novamente!", ex);
            }
        }
    }
}
