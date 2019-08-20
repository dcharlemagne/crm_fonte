using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Application.Plugin.itbc_benefdocanal
{
    public class ManagerPostEventAssync : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            if ((Stage)context.Stage != Stage.PostOperation)
                return;

            switch (EnumConverter<MessageName>(context.MessageName.ToLower()))
            {

                case MessageName.Update:
                    if (context.PostEntityImages.Contains("imagem")
                       && context.PostEntityImages["imagem"] is Entity
                       && context.PreEntityImages.Contains("imagem")
                       && context.PreEntityImages["imagem"] is Entity)
                    {
                        
                        Domain.Model.BeneficioDoCanal beneficioCanalPreImage = ((Entity)context.PreEntityImages["imagem"]).Parse<Domain.Model.BeneficioDoCanal>(context.OrganizationName, context.IsExecutingOffline, userService);
                        Domain.Model.BeneficioDoCanal beneficioCanalPostImage = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.BeneficioDoCanal>(context.OrganizationName, context.IsExecutingOffline, userService);

                        if (beneficioCanalPreImage.ContaObj.Classificacao.Name.Equals(Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Dist_VAD)
                                                                             || beneficioCanalPreImage.ContaObj.Classificacao.Name.Equals(Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Dist_BoxMover))
                        {
                            if (!beneficioCanalPreImage.StatusBeneficio.Id.Equals(beneficioCanalPostImage.StatusBeneficio.Id))
                            {
                                BeneficioService bservice = new BeneficioService(context.OrganizationName, context.IsExecutingOffline, userService);
                                var beneficios = bservice.ObterPor(beneficioCanalPostImage.Beneficio.Id);

                                if (beneficios.PassivelDeSolicitacao.Equals(true))
                                {
                                    Domain.Integracao.MSG0165 msgBenefCanal = new Domain.Integracao.MSG0165(context.OrganizationName, context.IsExecutingOffline);
                                    msgBenefCanal.Enviar(beneficioCanalPostImage);
                                }
                            }
                        }
                    }
                    break;
                case MessageName.SetStateDynamicEntity:

                    Domain.Model.BeneficioDoCanal beneficioCanalPostImage1 = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.BeneficioDoCanal>(context.OrganizationName, context.IsExecutingOffline, userService);

                    if (beneficioCanalPostImage1.ContaObj.Classificacao.Name.Equals(Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Dist_VAD)
                                                                             || beneficioCanalPostImage1.ContaObj.Classificacao.Name.Equals(Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Dist_BoxMover))
                    {

                        BeneficioService bs = new BeneficioService(context.OrganizationName, context.IsExecutingOffline, userService);
                        var beneficio = bs.ObterPor(beneficioCanalPostImage1.Beneficio.Id);

                        if (beneficio.PassivelDeSolicitacao.Equals(true))
                        {
                            Domain.Integracao.MSG0165 msgBenefCanal = new Domain.Integracao.MSG0165(context.OrganizationName, context.IsExecutingOffline);
                            msgBenefCanal.Enviar(beneficioCanalPostImage1);
                        }
                    }
                    
                    break;

            }        
        }
    }
}