using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;
using System;

namespace Intelbras.CRM2013.Application.Plugin.new_valor_servico_posto
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            switch (EnumConverter<MessageName>(context.MessageName.ToLower()))
            {
                #region Create

                case MessageName.Create:

                    var e = context.GetContextEntity();
                    var valor = e.Parse<ValorDoServicoPorPosto>(context.OrganizationName, context.IsExecutingOffline);
                    if (valor.SegmentoId == null && valor.ProdutoId == null)
                        throw new InvalidPluginExecutionException("Ação não executada. É necessário o preenchimentos de Produto ou Segmento.");
                    break;

                #endregion

                #region Update

                case MessageName.Update:

                    var entityMerge = context.PreEntityImages["imagem"];
                    var valorUpdate = entityMerge.Parse<ValorDoServicoPorPosto>(context.OrganizationName, context.IsExecutingOffline);
                    if (valorUpdate.SegmentoId == null && valorUpdate.ProdutoId == null)
                        throw new InvalidPluginExecutionException("Ação não executada. É necessário o preenchimentos de Produto ou Segmento.");

                    break;

                #endregion
            }
        }
    }
}
