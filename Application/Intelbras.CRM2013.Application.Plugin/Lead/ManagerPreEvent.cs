using System;
using Microsoft.Xrm.Sdk;
using SDKore.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelbras.CRM2013.Application.Plugin.ClientePotencial
{
    public class ManagerPreEvent : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var Context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);

            try
            {
                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(Context.MessageName))
                {
                    #region Create

                    case Domain.Enum.Plugin.MessageName.Create:

                        var e = (Entity)Context.InputParameters["Target"];

                        #region Salvar Numero Projeto
                        string numeroProjeto = "";
                        int ultimoNumeroProjeto;

                        ultimoNumeroProjeto = (new Domain.Servicos.RepositoryService(Context.OrganizationName, false)).ClientePotencial.ObterUltimoNumeroProjeto(DateTime.Now.Year.ToString());
                        ultimoNumeroProjeto += 1;
                        numeroProjeto = DateTime.Now.Year.ToString() + '-' + ultimoNumeroProjeto.ToString("0000000");

                        e.Attributes["itbc_numeroprojeto"] = numeroProjeto;

                        #endregion

                        break;

                    #endregion
                }
            }
            catch (Exception ex)
            {
                string mensagem = SDKore.Helper.Error.Handler(ex);
                throw new InvalidPluginExecutionException(mensagem);
            }
        }
    }
}
