using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Application.Plugin.itbc_famildeprod
{
    public class ManagerPostEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            {
                var ServiceFamiliaProduto = new Domain.Servicos.FamiliaProdutoService(context.OrganizationName, context.IsExecutingOffline, adminService);
                var repositoryService = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline, adminService);

                try
                {
                    Entity preImage = null;
                    Entity entidade = null;
                    switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                    {
                        case Domain.Enum.Plugin.MessageName.Update:

                            preImage = (Entity)context.PreEntityImages["imagem"];
                            entidade = (Entity)context.InputParameters["Target"];

                            if(entidade.Contains("itbc_desconto_verde_habilitado") && preImage.GetAttributeValue<bool>("itbc_desconto_verde_habilitado") && !entidade.GetAttributeValue<bool>("itbc_desconto_verde_habilitado"))
                            {
                                var listCanalVerde = repositoryService.CanalVerde.ListarPorFamilia(entidade.Id);

                                if(listCanalVerde.Count > 0)
                                {
                                    repositoryService.CanalVerde.InativarMultiplos(listCanalVerde, 1);
                                }
                            }
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
}
