using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Application.Plugin.itbc_solicitacaodebeneficio
{
    public class ManagerPostEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            switch (EnumConverter<MessageName>(context.MessageName.ToLower()))
            {
                #region Create

                case MessageName.Create:

                    if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                    {
                        var e = (Entity)context.InputParameters["Target"];
                        var SolBenef = e.Parse<Domain.Model.SolicitacaoBeneficio>(context.OrganizationName, context.IsExecutingOffline);
                        var ServiceSolicitacaoBeneficioCreate = new SolicitacaoBeneficioService(context.OrganizationName, context.IsExecutingOffline, userService);

                        ServiceSolicitacaoBeneficioCreate.SolicitarBaneficioPostCreate(SolBenef);
                        ServiceSolicitacaoBeneficioCreate.GerarTarefaSolicBeneficio(SolBenef, context.UserId, 1);
                    }

                    break;

                #endregion

                #region Update

                case MessageName.Update:

                    var posImage = context.PostEntityImages["imagem"];
                    var posSolicitacaoBeneficio = posImage.Parse<Domain.Model.SolicitacaoBeneficio>(context.OrganizationName, context.IsExecutingOffline, adminService);

                    var preImage = context.PreEntityImages["imagem"];
                    var preSolicitacaoBeneficio = preImage.Parse<Domain.Model.SolicitacaoBeneficio>(context.OrganizationName, context.IsExecutingOffline, adminService);

                    var ServiceSolicitacaoBeneficio = new SolicitacaoBeneficioService(context.OrganizationName, context.IsExecutingOffline, userService);
                    ServiceSolicitacaoBeneficio.SolicitarBeneficioPostUpdate(posSolicitacaoBeneficio, preSolicitacaoBeneficio);
                    break;

                #endregion
            }
        }
    }
}