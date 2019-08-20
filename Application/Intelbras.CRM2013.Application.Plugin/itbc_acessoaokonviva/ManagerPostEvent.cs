using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;
using System;
using Model = Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Application.Plugin.itbc_acessoaokonviva
{
    public class ManagerPostEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            if (context.GetStage() != Stage.PostOperation)
                return;

            var sAcessoKonviva = new AcessoKonvivaService(context.OrganizationName, context.IsExecutingOffline, null);

            switch (EnumConverter<MessageName>(context.MessageName.ToLower()))
            {
                case MessageName.Create:
                    {
                        var e = context.GetContextEntity();
                        var mAcessoKonviva = e.Parse<Model.AcessoKonviva>(context.OrganizationName, context.IsExecutingOffline);

                        if (context.SharedVariables.Contains("IntegraKonviva") && Convert.ToBoolean(context.SharedVariables["IntegraKonviva"]))
                        {
                            sAcessoKonviva.IntegracaoBarramento(mAcessoKonviva);
                        }
                        break;
                    }
                case MessageName.Update:
                    {
                        var acessoKonvivaImagem = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.AcessoKonviva>(context.OrganizationName, context.IsExecutingOffline, userService);

                        if (context.SharedVariables.Contains("IntegraKonviva") && Convert.ToBoolean(context.SharedVariables["IntegraKonviva"]))
                        {
                            sAcessoKonviva.IntegracaoBarramento(acessoKonvivaImagem);
                        }
                        break;
                    }
            }
        }
    }
}