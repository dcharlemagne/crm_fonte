using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Application.Plugin.itbc_portfoliokeyaccountrepresentantes
{
    public class ManagerPostEvent : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);
            var ServicePortfoliodoKeyAccountRepresentantes = new PortfoliodoKeyAccountRepresentantesService(context.OrganizationName, context.IsExecutingOffline, service);


            try
            {

                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {

                    #region Create
                    case Domain.Enum.Plugin.MessageName.Create:

                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                        {
                            var entidade = (Entity)context.InputParameters["Target"];

                            Domain.Model.PortfoliodoKeyAccountRepresentantes PortfolioRep = entidade.Parse<Domain.Model.PortfoliodoKeyAccountRepresentantes>(context.OrganizationName, context.IsExecutingOffline, service);

                            //Verifica se existe PortfolioKeyAccount duplicado
                            ServicePortfoliodoKeyAccountRepresentantes.VerificaDuplicidadePortforioKARepresentantes(PortfolioRep);

                            string lstResposta = new Domain.Servicos.PortfoliodoKeyAccountRepresentantesService
                                (context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(PortfolioRep);
                        }

                        break;

                    #endregion

                    #region Update

                    case Domain.Enum.Plugin.MessageName.Update:

                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            var portfolioRepPost = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.PortfoliodoKeyAccountRepresentantes>(context.OrganizationName, context.IsExecutingOffline, service);

                            Domain.Model.PortfoliodoKeyAccountRepresentantes _PortfolioKAPre = ((Entity)context.PreEntityImages["imagem"]).Parse<Domain.Model.PortfoliodoKeyAccountRepresentantes>(context.OrganizationName, context.IsExecutingOffline, service);
                            //Só para ter certeza que nao esta desativando ou ativando o registro
                            if (portfolioRepPost.State == null || portfolioRepPost.State == _PortfolioKAPre.State)
                            {
                                //Verifica se existe PortfolioKeyAccount duplicado
                                ServicePortfoliodoKeyAccountRepresentantes.VerificaDuplicidadePortforioKARepresentantes(portfolioRepPost);
                            }

                            string lstResposta = new Domain.Servicos.PortfoliodoKeyAccountRepresentantesService
                                (context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(portfolioRepPost);
                        }

                        break;

                    #endregion

                    #region Delete

                    case Domain.Enum.Plugin.MessageName.Delete:

                        if (context.PreEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            var portfolioRepPost = ((Entity)context.PreEntityImages["imagem"]).Parse<Domain.Model.PortfoliodoKeyAccountRepresentantes>(context.OrganizationName, context.IsExecutingOffline, service);

                            string lstResposta = new Domain.Servicos.PortfoliodoKeyAccountRepresentantesService
                                (context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(portfolioRepPost);
                        }

                        break;

                    #endregion

                    #region State
                    case Domain.Enum.Plugin.MessageName.SetStateDynamicEntity:
                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            //var portfolioRepPost = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.PortfolioRepresentante>(context.OrganizationName, context.IsExecutingOffline, service);
                            //string lstResposta = new Domain.Servicos.PortifolioRepresentanteService
                            //    (context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(portfolioRepPost);
                        }

                        break;
                    #endregion

                }

            }
            catch (Exception ex)
            {
                //trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), "Account", DateTime.Now));
                //trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

    }
}
