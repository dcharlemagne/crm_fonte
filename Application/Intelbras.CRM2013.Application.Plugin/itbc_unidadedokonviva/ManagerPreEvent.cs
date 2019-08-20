using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;
using System;

namespace Intelbras.CRM2013.Application.Plugin.itbc_unidadedokonviva
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            var e = context.GetContextEntity();

            Intelbras.CRM2013.Domain.Model.UnidadeKonviva mUnidadeKonviva = e.Parse<Intelbras.CRM2013.Domain.Model.UnidadeKonviva>(context.OrganizationName, context.IsExecutingOffline);
            DeParaDeUnidadeDoKonvivaService ServiceDeParaDeUnidadeDoKonviva = new DeParaDeUnidadeDoKonvivaService(context.OrganizationName, context.IsExecutingOffline, adminService);

            var targetEntity = (Entity)context.InputParameters["Target"];

            switch (EnumConverter<MessageName>(context.MessageName.ToLower()))
            {
                case MessageName.SetStateDynamicEntity:

                    var returnQuery = ServiceDeParaDeUnidadeDoKonviva.ObterDeParaPorUnidade(mUnidadeKonviva);


                    if(returnQuery.Count > 0)
                    {
                        throw new ApplicationException("Erro: Não é possivel inativar essa unidade. Ela esta sendo utilizada em um mapeamento de De Para de Acesso ao Konviva");
                    }

                    break;

            }
        }
    }
}