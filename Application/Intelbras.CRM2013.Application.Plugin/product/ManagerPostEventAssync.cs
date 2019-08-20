using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Model;
using System.Xml.Linq;
using SDKore.Configuration;

namespace Intelbras.CRM2013.Application.Plugin.product
{
    public class ManagerPostEventAssync : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            if (context.Depth > 2)
                return;

            switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
            {
                case Domain.Enum.Plugin.MessageName.Create:

                    Entity entidadeAlterada = (Entity)context.InputParameters["Target"];
                    Domain.Model.Product produto = entidadeAlterada.Parse<Domain.Model.Product>(context.OrganizationName, context.IsExecutingOffline);

                    //Inserir produto na lista de preço
                    new Intelbras.CRM2013.Domain.Servicos.ListaPrecoService(context.OrganizationName, context.IsExecutingOffline).Persistir(produto);

                    #region SellOut


                    //Verifica se o codigo do produto foi alterado
                    if (produto.Codigo != null)
                    {
                        string resposta;

                        XDocument xmlroot = new XDocument(
                        new XDeclaration("1.0", "utf-8", "no"),
                        new XElement("Produto",
                            new XElement("Idprodutocrm", produto.ID),
                            new XElement("Idprodutoerp", produto.Codigo),
                            new XElement("Statuscode", 1),
                            new XElement("Statecode", 0)
                            ));

                        string xml = xmlroot.Declaration.ToString() + Environment.NewLine + xmlroot.ToString(); ;

                        string usuario = SDKore.Configuration.ConfigurationManager.GetSettingValue("SellOutCRMWSUser");
                        string senha = SDKore.Configuration.ConfigurationManager.GetSettingValue("SellOutCRMWSPasswd");

                        bool resultado = new Domain.Servicos.SellOutService(context.OrganizationName,
                        context.IsExecutingOffline).PersistirProdutoSellOut(usuario, senha, xml, out resposta);

                        if (resultado == false)
                        {
                            throw new ArgumentException(resposta);
                        }
                    }
                    #endregion


                    break;

                case Domain.Enum.Plugin.MessageName.Update:
                    if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                    {
                        Entity entidadeAlterada1 = (Entity)context.InputParameters["Target"];
                        Entity entidadePost = (Entity)context.PostEntityImages["imagem"];
                        Domain.Model.Product produto1 = entidadeAlterada1.Parse<Domain.Model.Product>(context.OrganizationName, context.IsExecutingOffline);
                        Domain.Model.Product PostProduto = entidadePost.Parse<Domain.Model.Product>(context.OrganizationName, context.IsExecutingOffline);

                        if (produto1.Status.HasValue
                            && produto1.Status == (int?)Domain.Enum.Produto.StateCode.inativo
                            && produto1.RazaoStatus != null && (int)produto1.RazaoStatus == (int)Domain.Enum.Produto.StatusCode.Descontinuado)
                            new Domain.Servicos.ProdutoService(context.OrganizationName, context.IsExecutingOffline, serviceFactory).DescontinuarProduto(PostProduto);

                        if (produto1.IntegrarNoPlugin.HasValue && !produto1.IntegrarNoPlugin.Value)
                        {
                            string ret = new Domain.Servicos.ProdutoService(context.OrganizationName, context.IsExecutingOffline, serviceFactory).IntegracaoBarramento(PostProduto);
                        }

                        #region SellOut

                        //Verifica se o codigo do produto foi alterado
                        if (produto1.Codigo != null || produto1.RazaoStatus.HasValue)
                        {
                            string resposta;

                            XDocument xmlroot = new XDocument(
                            new XDeclaration("1.0", "utf-8", "no"),
                            new XElement("Produto",
                                new XElement("Idprodutocrm", PostProduto.ID),
                                new XElement("Idprodutoerp", PostProduto.Codigo),
                                new XElement("Statuscode", PostProduto.RazaoStatus),
                                new XElement("Statecode", PostProduto.Status)
                                ));

                            string xml = xmlroot.Declaration.ToString() + Environment.NewLine + xmlroot.ToString(); ;

                            string usuario = ConfigurationManager.GetSettingValue("SellOutCRMWSUser");
                            string senha = ConfigurationManager.GetSettingValue("SellOutCRMWSPasswd");

                            bool resultado = new Domain.Servicos.SellOutService(context.OrganizationName,
                            context.IsExecutingOffline).PersistirProdutoSellOut(usuario, senha, xml, out resposta);

                            if (resultado == false)
                            {
                                throw new ArgumentException(resposta);
                            }
                        }
                        #endregion
                    
                    }
                    break;

                case Domain.Enum.Plugin.MessageName.SetStateDynamicEntity:

                    if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity
                        && context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity)
                    {
                        var ProdutoPre = ((Entity)context.PreEntityImages["imagem"]).Parse<Domain.Model.Product>(context.OrganizationName, context.IsExecutingOffline, serviceFactory);
                        var ProdutoPos = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.Product>(context.OrganizationName, context.IsExecutingOffline, serviceFactory);


                        #region SellOut
                        if (ProdutoPre.Status != ProdutoPos.Status)
                        {
                            string resposta;

                            XDocument xmlroot = new XDocument(
                            new XDeclaration("1.0", "utf-8", "no"),
                            new XElement("Produto",
                                new XElement("Idprodutocrm", ProdutoPos.ID),
                                new XElement("Statecode", ProdutoPos.Status)
                                ));

                            string xml = xmlroot.Declaration.ToString() + Environment.NewLine + xmlroot.ToString(); ;

                            string usuario = SDKore.Configuration.ConfigurationManager.GetSettingValue("SellOutCRMWSUser");
                            string senha = SDKore.Configuration.ConfigurationManager.GetSettingValue("SellOutCRMWSPasswd");

                            bool resultado = new Domain.Servicos.SellOutService(context.OrganizationName,
                            context.IsExecutingOffline).PersistirProdutoSellOut(usuario, senha, xml, out resposta);

                            if (resultado == false)
                            {
                                throw new ArgumentException(resposta);
                            }
                        }

                        #endregion

                    }
                    break;
            }

        }
    }
}
