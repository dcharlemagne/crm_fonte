using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Application.Plugin.itbc_produtosdasolicitacao
{
    public class ManagerPostEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            var ServiceProdutosSolicitacao = new ProdutosdaSolicitacaoService(context.OrganizationName, context.IsExecutingOffline, userService);

            switch (EnumConverter<MessageName>(context.MessageName.ToLower()))
            {
                case MessageName.Create:
                case MessageName.Update:
                    var targetCreate = (Entity)context.InputParameters["Target"];
                    var produtosdaSolicitacaoCreate = targetCreate.Parse<ProdutosdaSolicitacao>(context.OrganizationName, context.IsExecutingOffline, userService);

                    ServiceProdutosSolicitacao.AtualizarValoresSolicitacao(produtosdaSolicitacaoCreate);
                    break;
                case MessageName.Delete:

                    var preImageDelete = context.PreEntityImages["imagem"];
                    var produtoSolicitacaoDelete = preImageDelete.Parse<ProdutosdaSolicitacao>(context.OrganizationName, context.IsExecutingOffline, adminService);

                    var SolicitacaoBeneficio = new SolicitacaoBeneficioService(context.OrganizationName, context.IsExecutingOffline, userService).ObterPor(produtoSolicitacaoDelete.SolicitacaoBeneficio.Id);
                    var ServiceSolicitacaoBeneficio = new SolicitacaoBeneficioService(context.OrganizationName, context.IsExecutingOffline, userService);
                    ServiceSolicitacaoBeneficio.Atualizar(SolicitacaoBeneficio);
                    break;

            }
        }
    }
}
