using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Application.Plugin.itbc_categoriasdocanal
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            var ServiceBeneficioDoCanal = new Intelbras.CRM2013.Domain.Servicos.BeneficioDoCanalService(context.OrganizationName, context.IsExecutingOffline);

            Domain.Model.ProdutoEstabelecimento prodEstabAtual = new Domain.Model.ProdutoEstabelecimento(context.OrganizationName, context.IsExecutingOffline);

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    case Domain.Enum.Plugin.MessageName.Create:
                        #region
                        var ent = (Entity)context.InputParameters["Target"];
                        Domain.Model.CategoriasCanal CatCanal = ent.Parse<Domain.Model.CategoriasCanal>(context.OrganizationName, context.IsExecutingOffline);

                        ServiceBeneficioDoCanal.VerificaDuplicidadeCategoria(CatCanal.Canal.Id, CatCanal.UnidadeNegocios.Id);

                        ServiceBeneficioDoCanal.VerificaConfiguracaoPerfilCategoria(CatCanal.Classificacao.Id, CatCanal.UnidadeNegocios.Id, CatCanal.Categoria.Id, CatCanal.Canal.Id);

                        // Insere o mesmo proprietário da Conta.
                        // Comentado devido problema com plugin do CRM2015 - não é possível alterar o OwnerId neste ponto - deixei comentado por enquanto
                        // var canalServices = new CategoriaCanalService(context.OrganizationName, context.IsExecutingOffline);
                        // ent = canalServices.AtribuiParaOProprietarioDoCanal(ent, CatCanal);

                        break;
                        #endregion
                    case Domain.Enum.Plugin.MessageName.Update:
                        #region
                        if (context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity)
                        {
                            var categoriasCanalPre = ((Entity)context.PreEntityImages["imagem"]).Parse<Domain.Model.CategoriasCanal>(context.OrganizationName, context.IsExecutingOffline);
                            var Entidade = (Entity)context.InputParameters["Target"];

                            Domain.Model.CategoriasCanal categoriasCanal = Entidade.Parse<Domain.Model.CategoriasCanal>(context.OrganizationName, context.IsExecutingOffline);

                            if (categoriasCanal.UnidadeNegocios != null)
                                categoriasCanalPre.UnidadeNegocios = new SDKore.DomainModel.Lookup(categoriasCanal.UnidadeNegocios.Id, "");

                            if (categoriasCanal.Canal != null)
                                categoriasCanalPre.Canal = new SDKore.DomainModel.Lookup(categoriasCanal.Canal.Id, "");

                            if (categoriasCanal.Categoria != null)
                                categoriasCanalPre.Categoria = new SDKore.DomainModel.Lookup(categoriasCanal.Categoria.Id, "");

                            if (categoriasCanal.Classificacao != null)
                                categoriasCanalPre.Classificacao = new SDKore.DomainModel.Lookup(categoriasCanal.Classificacao.Id, "");

                            if (categoriasCanalPre.UnidadeNegocios != null
                                && categoriasCanal.UnidadeNegocios != null
                                && categoriasCanalPre.Canal != null
                                && categoriasCanal.Canal != null)
                                if (categoriasCanalPre.UnidadeNegocios.Id != categoriasCanal.UnidadeNegocios.Id || categoriasCanalPre.Canal.Id != categoriasCanal.Canal.Id)
                                    ServiceBeneficioDoCanal.VerificaDuplicidadeCategoria(categoriasCanalPre.Canal.Id, categoriasCanalPre.UnidadeNegocios.Id);

                            if (categoriasCanal.Canal == null)
                                categoriasCanal.Canal = categoriasCanalPre.Canal;

                            // Ativando Categoria do Canal
                            if (categoriasCanal.Status.HasValue
                                && (categoriasCanalPre.Status == (int)Domain.Enum.CategoriaCanal.StateCode.Desativado
                                && categoriasCanal.Status.Value == (int)Domain.Enum.CategoriaCanal.StateCode.Ativado))
                            {
                                ServiceBeneficioDoCanal.VerificaConfiguracaoPerfilCategoria(categoriasCanalPre.Classificacao.Id, categoriasCanalPre.UnidadeNegocios.Id, categoriasCanalPre.Categoria.Id, categoriasCanal.Canal.Id);
                            }
                        }

                        break;
                        #endregion
                }
            }
        }
    }
}