using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Application.Plugin.itbc_segmento
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
                    Entity postImage = null;
                    Entity entidade = null;
                    switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                    {
                        case Domain.Enum.Plugin.MessageName.Update:

                            preImage = (Entity)context.PreEntityImages["imagem"];
                            postImage = (Entity)context.PostEntityImages["imagem"];
                            entidade = (Entity)context.InputParameters["Target"];

                            if ((entidade.Contains("itbc_desconto_verde_habilitado") && entidade.GetAttributeValue<bool>("itbc_desconto_verde_habilitado") && entidade.GetAttributeValue<bool>("itbc_desconto_verde_habilitado") != preImage.GetAttributeValue<bool>("itbc_desconto_verde_habilitado"))
                                || (entidade.Contains("itbc_percentual_desconto_verde") && preImage.GetAttributeValue<bool>("itbc_desconto_verde_habilitado") && entidade.GetAttributeValue<decimal>("itbc_percentual_desconto_verde") != preImage.GetAttributeValue<decimal>("itbc_percentual_desconto_verde"))
                                )
                            {
                                var listFamiliaComercial = ServiceFamiliaProduto.ListarPorSegmento(entidade.Id, false, null, null);
                                var listaUpdate = new List<FamiliaProduto>();
                                foreach (var famComTmp in listFamiliaComercial)
                                {
                                    famComTmp.DescontoVerdeHabilitado = postImage.GetAttributeValue<bool>("itbc_desconto_verde_habilitado");
                                    famComTmp.PercentualDescontoVerde = postImage.GetAttributeValue<decimal>("itbc_percentual_desconto_verde");
                                    ServiceFamiliaProduto.Persistir(famComTmp);
                                }

                            }

                            if(entidade.Contains("itbc_desconto_verde_habilitado") && preImage.GetAttributeValue<bool>("itbc_desconto_verde_habilitado") && !entidade.GetAttributeValue<bool>("itbc_desconto_verde_habilitado"))
                            {
                                var listCanalVerde = repositoryService.CanalVerde.ListarPorSegmento(entidade.Id);

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
