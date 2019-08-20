using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Application.Plugin.itbc_benefdocanal
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            var e = context.GetContextEntity();
            BeneficioDoCanalService ServiceBeneficioCanal = new BeneficioDoCanalService(context.OrganizationName, context.IsExecutingOffline, userService);

            switch (EnumConverter<MessageName>(context.MessageName.ToLower()))
            {
                case MessageName.Create:
                    {
                        Domain.Model.BeneficioDoCanal beneficioCanalCreate = e.Parse<Domain.Model.BeneficioDoCanal>(context.OrganizationName, context.IsExecutingOffline, userService);

                        switch ((Stage)context.Stage)
                        {
                            case Stage.PreOperation:
                                // Comentado devido problema com plugin do CRM2015 - não é possível alterar o OwnerId neste ponto - deixei comentado por enquanto
                                //e = ServiceBeneficioCanal.AtribuiParaOProprietarioDoCanal(e, beneficioCanalCreate);
                                break;
                        }
                        break;
                    }
            }
        }
    }
}