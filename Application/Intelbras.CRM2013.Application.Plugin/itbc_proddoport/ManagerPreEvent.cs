using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;
using System;

namespace Intelbras.CRM2013.Application.Plugin.itbc_proddoport
{
    public class ManagerPreEvent : IPlugin
    {
        private ProdutoPortfolio GetPreImage(IPluginExecutionContext context, IOrganizationService service)
        {
            if (context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity)
            {
                var entity = (Entity)context.PreEntityImages["imagem"];
                return entity.Parse<ProdutoPortfolio>(context.OrganizationName, context.IsExecutingOffline, service);
            }

            throw new ApplicationException("(Configuração) Pré-Image não registrada no plug-in.");
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = serviceFactory.CreateOrganizationService(null);
            var ServicePortfolio = new Intelbras.CRM2013.Domain.Servicos.PortfolioService(context.OrganizationName, context.IsExecutingOffline);

            try
            {
                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    #region Create

                    case Domain.Enum.Plugin.MessageName.Create:

                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                        {
                            var entidade = (Entity)context.InputParameters["Target"];
                            Domain.Model.ProdutoPortfolio ProdPortfolio = entidade.Parse<Domain.Model.ProdutoPortfolio>(context.OrganizationName, context.IsExecutingOffline, service);

                            ServicePortfolio.VerificaProdutoDuplicado(ProdPortfolio);

                            ServicePortfolio.VerificaCrossSellingSolucao(ProdPortfolio);

                            ServicePortfolio.VerificaVinculoProdutoVsProdutoPortifolio(ProdPortfolio);
                            ServicePortfolio.VerificaVinculoPortifolio(ProdPortfolio);
                        }

                        break;

                    #endregion

                    #region Update

                    case Domain.Enum.Plugin.MessageName.Update:

                        if (context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity)
                        {
                            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                            {
                                var entidade = (Entity)context.InputParameters["Target"];

                                if (!entidade.Contains("statecode"))
                                {

                                    var target = entidade.Parse<ProdutoPortfolio>(context.OrganizationName, context.IsExecutingOffline, service);
                                    var preImege = GetPreImage(context, service);

                                    ServicePortfolio.VerificaVinculoProdutoVsProdutoPortifolioAlteracao(target, preImege);

                                    ServicePortfolio.VerificaVinculoPortifolio(target);

                                    ServicePortfolio.VerificaVinculoPortifolioCrossSelling(preImege);

                                    ServicePortfolio.VerificaVinculoPortifolioSolucao(preImege);

                                }
                            }
                        }
                        break;

                    #endregion
                  
                    #region SetStateDynamicEntity

                    case Domain.Enum.Plugin.MessageName.SetStateDynamicEntity:

                        if (context.InputParameters.Contains("EntityMoniker") && context.InputParameters["EntityMoniker"] is EntityReference)
                        {
                            var state = (OptionSetValue)context.InputParameters["State"];
                            var status = (OptionSetValue)context.InputParameters["Status"];

                            var portContext = context.GetContextEntity("imagem");
                            var prodPortifolio = portContext.Parse<Domain.Model.ProdutoPortfolio>(context.OrganizationName, context.IsExecutingOffline);

                            if (prodPortifolio.Portfolio.Name.Contains("Normal"))
                            {
                                if (state.Value != (int)Domain.Enum.ProdutoPortfolio.StateCode.Ativo)
                                {
                                    ServicePortfolio.VerificaVinculoPortifolioCrossSelling(prodPortifolio);
                                    ServicePortfolio.VerificaVinculoPortifolioSolucao(prodPortifolio);
                                }
                            }
                            else if (prodPortifolio.Portfolio.Name.Contains("Cross-Selling") || prodPortifolio.Portfolio.Name.Contains("Solucao"))
                            {
                                ServicePortfolio.VerificaCrossSellingSolucao(prodPortifolio);
                            }
                            
                           
                            if (state.Value == (int)Domain.Enum.ProdutoPortfolio.StateCode.Ativo)
                            {
                                var preImage = GetPreImage(context, service);
                                preImage.Status = status.Value;

                                ServicePortfolio.VerificaVinculoPortifolio(preImage);                              
                            }
                        }
                        break;

                    #endregion
                }
            }
            catch (Exception ex)
            {
                string message = SDKore.Helper.Error.Handler(ex);

                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(message);
            }
        }
    }
}