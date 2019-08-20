using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Application.Plugin.itbc_psdid
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            var ServiceListaPSD = new Domain.Servicos.ListaPSDService(context.OrganizationName, context.IsExecutingOffline, adminService);

            try
            {
                Entity _target = null;
                Entity preImage = null;

                Domain.Model.ListaPrecoPSDPPPSCF targetLstPsd = ((Entity)context.InputParameters["Target"]).Parse<Domain.Model.ListaPrecoPSDPPPSCF>(context.OrganizationName, context.IsExecutingOffline);

                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    case Domain.Enum.Plugin.MessageName.Create:
                        if (ServiceListaPSD.ValidarExistencia(targetLstPsd, new List<Guid>()))
                            throw new ArgumentException("(CRM) Registro de Lista PSD já existe com as mesmas características.");
                        break;
                    case Domain.Enum.Plugin.MessageName.Update:
                        preImage = (Entity)context.PreEntityImages["imagem"];
                        _target = (Entity)context.InputParameters["Target"];

                        Domain.Model.ListaPrecoPSDPPPSCF lstPsd = ((Entity)context.PreEntityImages["imagem"]).Parse<Domain.Model.ListaPrecoPSDPPPSCF>(context.OrganizationName, context.IsExecutingOffline);

                        if(targetLstPsd.State.HasValue && targetLstPsd.State.Value == (int)Domain.Enum.ListaPsd.State.Ativo)
                            if (ServiceListaPSD.ValidarExistencia(lstPsd, new List<Guid>()))
                                throw new ArgumentException("(CRM) Registro de Lista PSD já existe com as mesmas características.");
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(SDKore.Helper.Error.GetMessageError(ex));
            }
        }
    }
}
