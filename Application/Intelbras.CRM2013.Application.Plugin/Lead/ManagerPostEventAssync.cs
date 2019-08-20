using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain;
using Microsoft.Xrm.Sdk.Client;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Application.Plugin.ClientePotencial
{
    public class ManagerPostEventAssync : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {

            switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
            {
                case Domain.Enum.Plugin.MessageName.Create:
                    
                    var entidade = context.GetContextEntity(); 
                    Domain.Model.ClientePotencial ClientePotencial = entidade.Parse<Domain.Model.ClientePotencial>(context.OrganizationName, context.IsExecutingOffline, adminService);
                    var leadService = new Domain.Servicos.LeadService(context.OrganizationName, context.IsExecutingOffline);

                    #region Sharepoint
                    string diretorio = "";

                    if ((!string.IsNullOrEmpty(ClientePotencial.PrimeiroNomeDoContato)) && (!string.IsNullOrEmpty(ClientePotencial.SobreNomeDoContato)))
                    {
                        diretorio = ClientePotencial.PrimeiroNomeDoContato + " " + ClientePotencial.SobreNomeDoContato;
                    }else if (!string.IsNullOrEmpty(ClientePotencial.NumeroProjeto))
                    {
                        diretorio = ClientePotencial.NumeroProjeto;
                    }
                    
                    if (ClientePotencial != null && !string.IsNullOrEmpty(diretorio) && !string.IsNullOrWhiteSpace(diretorio))
                        new SharepointServices(context.OrganizationName, context.IsExecutingOffline, adminService).CriarDiretorio<Domain.Model.ClientePotencial>(diretorio, ClientePotencial.ID.Value);

                    #endregion

                    #region Envia email caso ja exista projeto igual
                    if (leadService.ListarProjetosDuplicidade(ClientePotencial.Cnpj, ClientePotencial.UnidadeNegocio.Id.ToString()).Count > 1)
                    {
                        (new RepositoryService(context.OrganizationName, context.IsExecutingOffline)).ClientePotencial.EnviaEmailRegistroProjeto(ClientePotencial, true);
                    }else
                    {
                        (new RepositoryService(context.OrganizationName, context.IsExecutingOffline)).ClientePotencial.EnviaEmailRegistroProjeto(ClientePotencial, false);
                    }
                    #endregion

                    break;
            }
        }
    }
}
