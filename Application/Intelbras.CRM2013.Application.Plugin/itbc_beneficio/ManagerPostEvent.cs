using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;
using SDKore.DomainModel;
using System.Reflection;

namespace Intelbras.CRM2013.Application.Plugin.itbc_beneficio
{
    public class ManagerPostEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {

            try
            {
                var e = context.GetContextEntity();

                Intelbras.CRM2013.Domain.Model.Beneficio mBeneficio = e.Parse<Intelbras.CRM2013.Domain.Model.Beneficio>(context.OrganizationName, context.IsExecutingOffline);
                BeneficioService ServiceSolicitacaoBeneficio = new BeneficioService(context.OrganizationName, context.IsExecutingOffline, adminService);

                switch (EnumConverter<MessageName>(context.MessageName.ToLower()))
                {
                    case MessageName.Update:
                        {
                            switch ((Stage)context.Stage)
                            {
                                case Stage.PostOperation:
                                    ServiceSolicitacaoBeneficio.AtivarOrDesativado(mBeneficio);
                                    break;
                            }
                            break;
                        }
                }
            }
            catch (Exception erro)
            {
                throw new InvalidPluginExecutionException(erro.Message);
            }

        }
    }
}
