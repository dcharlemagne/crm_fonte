using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;
using SDKore.DomainModel;
namespace Intelbras.CRM2013.Application.Plugin.itbc_compromissos
{
    public class ManagerPostEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            var e = context.GetContextEntity();
            Intelbras.CRM2013.Domain.Model.CompromissosDoPrograma mCompromissoPrograma = e.Parse<Intelbras.CRM2013.Domain.Model.CompromissosDoPrograma>(context.OrganizationName, context.IsExecutingOffline, userService);
            CompromissosDoCanalService ServiceCompromissosDoCanal = new CompromissosDoCanalService(context.OrganizationName, context.IsExecutingOffline, userService);
            ParametroGlobalService ServiceParametroGlobal = new ParametroGlobalService(context.OrganizationName, context.IsExecutingOffline, userService);

            switch (EnumConverter<MessageName>(context.MessageName.ToLower()))
            {
                case MessageName.Update:
                    {
                        switch ((Stage)context.Stage)
                        {
                            case Stage.PostOperation:
                                if (mCompromissoPrograma.TipoMonitoramento == (int)Domain.Enum.CompromissoPrograma.TipoMonitoramento.PorTarefas)
                                {
                                    Domain.Model.ParametroGlobal paramGlobal = ServiceParametroGlobal.ObterFrequenciaAtividadeChecklist(mCompromissoPrograma.ID.Value);

                                    if (paramGlobal == null || paramGlobal.Valor == null)
                                        throw new ApplicationException("Operação cancelada. Parâmetro global de frequência de checkList não existe para este compromisso.");
                                }
                                break;
                        }
                    }
                    break;
            }
        }
    }
}