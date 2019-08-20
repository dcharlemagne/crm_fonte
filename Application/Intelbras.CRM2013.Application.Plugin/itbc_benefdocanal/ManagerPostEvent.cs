using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Application.Plugin.itbc_benefdocanal
{
    public class ManagerPostEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            if ((Stage)context.Stage != Stage.PostOperation)
                return;


            switch (EnumConverter<MessageName>(context.MessageName.ToLower()))
            {
                case MessageName.Update:

                    if (!context.PostEntityImages.Contains("imagem"))
                    {
                        throw new InvalidPluginExecutionException("(CRM) É necessário registrar uma Pre Image para esse plugin!");
                    }

                    Entity target = (Entity)context.InputParameters["Target"];
                    Domain.Model.BeneficioDoCanal beneficioCanalPostImage = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.BeneficioDoCanal>(context.OrganizationName, context.IsExecutingOffline, userService);

                    // Inativa o Canal caso Verba = 0 e Canal não participante.
                    if (beneficioCanalPostImage.Status.HasValue
                        && beneficioCanalPostImage.Status.Value == (int)Domain.Enum.BeneficioDoCanal.StateCode.Ativo)
                    {
                        if (!beneficioCanalPostImage.VerbaBrutaPeriodoAtual.HasValue
                            || beneficioCanalPostImage.VerbaBrutaPeriodoAtual.Value == 0)
                        {
                            BeneficioDoCanalService beneficioDoCanalService = new BeneficioDoCanalService(context.OrganizationName, context.IsExecutingOffline, userService);
                            beneficioDoCanalService.InativaBeneficioDoCanal(beneficioCanalPostImage);
                        }
                    }
                    break;

                case MessageName.Create:

                    if (!context.PostEntityImages.Contains("imagem"))
                    {
                        throw new InvalidPluginExecutionException("(CRM) É necessário registrar uma Post Image para esse plugin!");
                    }

                    Domain.Model.BeneficioDoCanal beneficioCanalPost = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.BeneficioDoCanal>(context.OrganizationName, context.IsExecutingOffline, userService);

                    if (beneficioCanalPost.ContaObj.Classificacao.Name.Equals(Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Dist_VAD)
                                                     || beneficioCanalPost.ContaObj.Classificacao.Name.Equals(Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Dist_BoxMover))
                    {
                        Domain.Integracao.MSG0165 msgProdEstab = new Domain.Integracao.MSG0165(context.OrganizationName, context.IsExecutingOffline);
                        msgProdEstab.Enviar(beneficioCanalPost);
                    }
                    break;
            }
        }
    }
}