using System;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Application.Plugin.itbc_deparadeunidadedokonviva
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {

            var deParaService = new Intelbras.CRM2013.Domain.Servicos.DeParaDeUnidadeDoKonvivaService(context.OrganizationName, context.IsExecutingOffline, adminService);

            switch (Util.Utilitario.ConverterEnum<MessageName>(context.MessageName))
            {
                case MessageName.Create:
                    if (context.InputParameters.Contains("Target"))
                    {
                        var e = (Entity)context.InputParameters["Target"];
                        //var dePara = e.Parse<Domain.Model.DeParaDeUnidadeDoKonviva>(context.OrganizationName, context.IsExecutingOffline, adminService);

                        if (deParaService.RegistroDuplicado(e.Attributes.ToList()))
                            throw new ArgumentException("(CRM) Já Existe um registro “De/Para de Unidade do Konviva” com esse mesmo conjunto de Paramêtros.");
                    }
                    break;
                case MessageName.Update:
                    if (context.InputParameters.Contains("Target"))
                    {
                        var e = (Entity)context.InputParameters["Target"];
                        var image = (Entity)context.PreEntityImages["imagem"];
                        image = UpdateImage(image, e);

                        if (deParaService.RegistroDuplicado(image.Attributes.ToList()))
                            throw new ArgumentException("(CRM) Já Existe um registro “De/Para de Unidade do Konviva” com esse mesmo conjunto de Paramêtros.");
                    }
                    break;
            }
        }
    }
}
