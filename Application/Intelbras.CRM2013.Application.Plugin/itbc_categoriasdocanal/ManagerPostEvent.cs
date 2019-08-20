using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Application.Plugin.itbc_categoriasdocanal
{
    public class ManagerPostEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
            {
                case Domain.Enum.Plugin.MessageName.Create:

                    if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                    {
                        var target = (Entity)context.InputParameters["Target"];
                        var entidade = target.Parse<Domain.Model.CategoriasCanal>(context.OrganizationName, context.IsExecutingOffline, adminService);
                        new BeneficioDoCanalService(context.OrganizationName, context.IsExecutingOffline, adminService).AdesaoAoProgramaNovaCategoria(entidade);

                        if (entidade.ContaObj.Classificacao.Name.Equals(Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Dist_VAD)
                                                     || entidade.ContaObj.Classificacao.Name.Equals(Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Dist_BoxMover))
                        {
                            //Remover dos plugins de Categoria do Canal o envio da mensagem MSG0122.
                            string xmlResposta = new BeneficioDoCanalService(context.OrganizationName,
                                context.IsExecutingOffline).IntegracaoBarramento(entidade);
                        }                        
                    }                    

                    break;

                case Domain.Enum.Plugin.MessageName.Update:

                    if (context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity &&
                        context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                    {

                        var CategoriaPre = ((Entity)context.PreEntityImages["imagem"]).Parse<Domain.Model.CategoriasCanal>(context.OrganizationName, context.IsExecutingOffline, adminService);
                        var CategoriaPost = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.CategoriasCanal>(context.OrganizationName, context.IsExecutingOffline, adminService);

                        //Se estiver desativando a categoria
                        if (CategoriaPre.Status.Value.Equals((int)Domain.Enum.CategoriaCanal.StateCode.Ativado)
                            && CategoriaPost.Status.Value.Equals((int)Domain.Enum.CategoriaCanal.StateCode.Desativado))
                        {
                            var categoriaCanalService = new CategoriaCanalService(context.OrganizationName, context.IsExecutingOffline);

                            categoriaCanalService.InativarCategoriaDoCanal(CategoriaPost);
                        }
                        //Se estiver Ativando a categoria
                        if (CategoriaPre.Status.Value.Equals((int)Domain.Enum.CategoriaCanal.StateCode.Desativado)
                            && CategoriaPost.Status.Value.Equals((int)Domain.Enum.CategoriaCanal.StateCode.Ativado))
                        {
                            new BeneficioDoCanalService(context.OrganizationName, context.IsExecutingOffline, adminService).AdesaoAoProgramaNovaCategoria(CategoriaPost);
                        }

                        if (CategoriaPost.ContaObj.Classificacao.Name.Equals(Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Dist_VAD)
                                                     || CategoriaPost.ContaObj.Classificacao.Name.Equals(Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Dist_BoxMover))
                        {
                            //Remover dos plugins de Categoria do Canal o envio da mensagem MSG0122.
                            string xmlResposta = new BeneficioDoCanalService(context.OrganizationName,
                                context.IsExecutingOffline, adminService).IntegracaoBarramento(CategoriaPost);
                        }
                    }
                    break;
            }
        }
    }
}