using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;
using System;

namespace Intelbras.CRM2013.Application.Plugin.new_extrato_logistica_reversa
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            switch (EnumConverter<MessageName>(context.MessageName.ToLower()))
            {
                #region Delete

                case MessageName.Delete:
                    var entidade = context.PreEntityImages["imagem"];
                    new RepositoryService(context.OrganizationName, context.IsExecutingOffline).Diagnostico.LimparCampoExtratoLogisticaReversa(entidade.Id);
                    break;

                #endregion

            }
        }
    }
}
