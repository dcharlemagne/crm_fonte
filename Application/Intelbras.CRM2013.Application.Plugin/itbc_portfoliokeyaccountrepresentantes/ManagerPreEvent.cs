using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Application.Plugin.itbc_portfoliokeyaccountrepresentantes
{
    public class ManagerPreEvent : IPlugin
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
                var entidade = (Entity)context.InputParameters["Target"];
                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    case Domain.Enum.Plugin.MessageName.Create:

                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                        {

                            //PortfoliodoKeyAccountRepresentantes PortfolioKA = entidade.Parse<Domain.Model.PortfoliodoKeyAccountRepresentantes>(context.OrganizationName, context.IsExecutingOffline, service);

                            ////Verifica se existe PortfolioKeyAccount duplicado
                            //ServicePortfoliodoKeyAccountRepresentantes.VerificaDuplicidadePortforioKARepresentantes(PortfolioKA);

                        }

                        break;

                    case Domain.Enum.Plugin.MessageName.Update:

                        if (context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity)
                        {                            
                            //Verificamos se o parentContext é nulo para ter certeza que é uma ação de update,caso for de delete/setState etc ele nao precisa verificar a duplicidade
                            if (context.ParentContext.MessageName.Equals(Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName).ToString()))
                            {
                                Domain.Model.PortfoliodoKeyAccountRepresentantes _PortfolioKAPre = ((Entity)context.PreEntityImages["imagem"]).Parse<Domain.Model.PortfoliodoKeyAccountRepresentantes>(context.OrganizationName, context.IsExecutingOffline, service);
                                Domain.Model.PortfoliodoKeyAccountRepresentantes _PortfolioKA = entidade.Parse<Domain.Model.PortfoliodoKeyAccountRepresentantes>(context.OrganizationName, context.IsExecutingOffline, service);

                                //Só para ter certeza que nao esta desativando ou ativando o registro
                                if (_PortfolioKA.State == null || _PortfolioKAPre.State == _PortfolioKA.State)
                                {
                                    if (!entidade.Contains("itbc_contatoid")) _PortfolioKA.KeyAccountRepresentante = _PortfolioKAPre.KeyAccountRepresentante;
                                    if (!entidade.Contains("itbc_unidadedenegocioid")) _PortfolioKA.UnidadedeNegocio = _PortfolioKAPre.UnidadedeNegocio;
                                    if (!entidade.Contains("itbc_segmentoid")) _PortfolioKA.Segmento = _PortfolioKAPre.Segmento;
                                    if (!entidade.Contains("itbc_supervisordevendas")) _PortfolioKA.SupervisordeVendas = _PortfolioKAPre.SupervisordeVendas;
                                    if (!entidade.Contains("itbc_assistentedeadministracaodevendas")) _PortfolioKA.AssistentedeAdministracaodeVendas = _PortfolioKAPre.AssistentedeAdministracaodeVendas;

                                    //Verifica se existe PortfolioKeyAccount duplicado
                                    //ServicePortfoliodoKeyAccountRepresentantes.VerificaDuplicidadePortforioKARepresentantes(_PortfolioKA);
                                }
                            }
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), @"Categoria", DateTime.Now));
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}