using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;
using SDKore.DomainModel;
using System.Reflection;

namespace Intelbras.CRM2013.Application.Plugin.itbc_metaunidade
{
    public class ManagerPreEvent : PluginBase
    {

        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            var e = context.GetContextEntity();
            Intelbras.CRM2013.Domain.Model.MetadaUnidade mMetas = e.Parse<Intelbras.CRM2013.Domain.Model.MetadaUnidade>(context.OrganizationName, context.IsExecutingOffline);
            MetadaUnidadeService ServiceMetadaUnidade = new MetadaUnidadeService(context.OrganizationName, context.IsExecutingOffline);


            switch (EnumConverter<MessageName>(context.MessageName.ToLower()))
            {
                case MessageName.Create:
                    {
                        switch ((Stage)context.Stage)
                        {
                            case Stage.PreOperation:
                                ServiceMetadaUnidade.PreCreate(mMetas);
                                break;
                        }
                        break;
                    }
            }
        }
    }
}