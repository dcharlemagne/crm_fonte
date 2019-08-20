using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using System.Xml.Linq;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Application.Plugin.itbc_itbc_prodestabelecimento
{
    public class ManagerPostEvent : IPlugin
    {

        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);

            

            try
            {

                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {

                    #region Create
                    case Domain.Enum.Plugin.MessageName.Create:

                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                        {
                            var entidade = (Entity)context.InputParameters["Target"];
                            Domain.Model.ProdutoEstabelecimento produtoEstabelecimento = entidade.Parse<Domain.Model.ProdutoEstabelecimento>(context.OrganizationName, context.IsExecutingOffline, service);
                            string lstResposta = new Domain.Servicos.ProdutoEstabelecimentoService(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(produtoEstabelecimento);
                        }

                        break;

                    #endregion

                    #region Update

                    case Domain.Enum.Plugin.MessageName.Update:

                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            var prodEstabPost = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.ProdutoEstabelecimento>(context.OrganizationName, context.IsExecutingOffline, service);
                            string lstResposta = new Domain.Servicos.ProdutoEstabelecimentoService(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(prodEstabPost);
                        }

                        break;

                    #endregion

                    #region Delete

                    case Domain.Enum.Plugin.MessageName.Delete:

                        if (context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity)
                        {
                            var prodEstabPost = ((Entity)context.PreEntityImages["imagem"]).Parse<Domain.Model.ProdutoEstabelecimento>(context.OrganizationName, context.IsExecutingOffline, service);
                            string lstResposta = new Domain.Servicos.ProdutoEstabelecimentoService(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramentoDelete(prodEstabPost);
                        }
                        break;

                    #endregion
                }

            }
            catch (Exception ex)
            {
                trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), "Account", DateTime.Now));
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }


    }
}
