using Microsoft.Xrm.Sdk;
using SDKore.Configuration;
using System;
using System.Xml.Linq;

namespace Intelbras.CRM2013.Application.Plugin.product
{
    public class ManagerPostEvent : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            if (context.Depth > 2)
                return;

            try
            {
                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    case Domain.Enum.Plugin.MessageName.Create:
                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                        {
                            Entity entidadeAlterada = (Entity)context.InputParameters["Target"];
                            Domain.Model.Product produto = entidadeAlterada.Parse<Domain.Model.Product>(context.OrganizationName, context.IsExecutingOffline);

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
                                    new XElement("Statuscode", produto.RazaoStatus),
                                    new XElement("Statecode", produto.Status),
                                    new XElement("DescricaoProduto", produto.Descricao)
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

                    case Domain.Enum.Plugin.MessageName.Update:

                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                        {
                            Entity entidadeAlterada = (Entity)context.InputParameters["Target"];
                            Entity entidadePost = (Entity)context.PostEntityImages["imagem"];
                            Domain.Model.Product produto = entidadeAlterada.Parse<Domain.Model.Product>(context.OrganizationName, context.IsExecutingOffline);
                            Domain.Model.Product PostProduto = entidadePost.Parse<Domain.Model.Product>(context.OrganizationName, context.IsExecutingOffline);

                            if (produto.Status.HasValue
                                && produto.Status == (int?)Domain.Enum.Produto.StateCode.inativo
                                && produto.RazaoStatus != null && (int)produto.RazaoStatus == (int)Domain.Enum.Produto.StatusCode.Descontinuado)
                                new Domain.Servicos.ProdutoService(context.OrganizationName, context.IsExecutingOffline, service).DescontinuarProduto(PostProduto);

                            if (produto.IntegrarNoPlugin.HasValue && !produto.IntegrarNoPlugin.Value)
                            {
                                string ret = new Domain.Servicos.ProdutoService(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(PostProduto);
                            }

                            #region SellOut

                            //Verifica se o codigo do produto foi alterado
                            if (produto.Codigo != null || produto.RazaoStatus.HasValue)
                            {
                                string resposta;

                                XDocument xmlroot = new XDocument(
                                new XDeclaration("1.0", "utf-8", "no"),
                                new XElement("Produto",
                                    new XElement("Idprodutocrm", PostProduto.ID),
                                    new XElement("Idprodutoerp", PostProduto.Codigo),
                                    new XElement("Statuscode", PostProduto.RazaoStatus),
                                    new XElement("Statecode", PostProduto.Status),
                                    new XElement("DescricaoProduto", PostProduto.Descricao)
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
                            var ProdutoPre = ((Entity)context.PreEntityImages["imagem"]).Parse<Domain.Model.Product>(context.OrganizationName, context.IsExecutingOffline, service);
                            var ProdutoPos = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.Product>(context.OrganizationName, context.IsExecutingOffline, service);


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
            catch (Exception ex)
            {
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));

                string message = SDKore.Helper.Error.Handler(ex);
                throw new InvalidPluginExecutionException(message, ex);
            }
        }
    }

}