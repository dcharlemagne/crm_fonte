using Microsoft.Xrm.Sdk;
using System;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.ViewModels;
using SDKore.Helper;

namespace Intelbras.CRM2013.Application.Plugin.CustomerAddress
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            try
            {
                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    #region Create
                    case Domain.Enum.Plugin.MessageName.Create:

                        var parameter = (Entity)context.InputParameters["Target"];
                        var endereco = parameter.Parse<Endereco>(context.OrganizationName, context.IsExecutingOffline, adminService);
                        var contextCreate = context.GetContextEntity();

                        preencheDados(endereco, ref context, adminService);

                        break;
                    #endregion

                    #region Update
                    case Domain.Enum.Plugin.MessageName.Update:

                        var targetUpdate = (Entity)context.InputParameters["Target"];
                        var enderecoUpdate = targetUpdate.Parse<Endereco>(context.OrganizationName, context.IsExecutingOffline, adminService);
                        var entidadeComValoresFinais = (Entity)context.GetContextEntityMerge("imagem");

                        enderecoUpdate.IntegrarNoPlugin = entidadeComValoresFinais.GetAttributeValue<bool>("itbc_acaocrm");
                        enderecoUpdate.Identificacao = entidadeComValoresFinais.GetAttributeValue<string>("new_cnpj").GetOnlyNumbers();
                        preencheDados(enderecoUpdate, ref context, adminService);

                        break;
                    #endregion

                    #region Delete
                    case Domain.Enum.Plugin.MessageName.Delete:

                        var image = context.PreEntityImages["imagem"];
                        var enderecoDelete = image.Parse<Endereco>(context.OrganizationName, context.IsExecutingOffline, adminService);

                        enderecoDelete.StatusAtivo = false;
                        if (enderecoDelete.IntegrarNoPlugin)
                        {
                            string xmlResposta = new EnderecoService(context.OrganizationName, context.IsExecutingOffline, adminService).IntegracaoBarramento(enderecoDelete);
                        }

                        break;
                        #endregion
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        private void preencheDados(Endereco endereco, ref IPluginExecutionContext context, IOrganizationService adminService)
        {
            var target = context.GetContextEntity();
            target.Attributes["new_cnpj"] = endereco.Identificacao.GetOnlyNumbers();
            // Caso de exclusão do registro não busca a mensagem de CEP novamente
            if (endereco.IntegrarNoPlugin && endereco.Cep != null)
            {
                var service = new EnderecoServices(context.OrganizationName, context.IsExecutingOffline, adminService);

                CepViewModel enderecoIntegracao = service.BuscaCep(endereco.Cep.GetOnlyNumbers());
                if (enderecoIntegracao == null)
                {
                    throw new ArgumentException("(CRM) Este CEP não foi encontrado na base do Totvs.");
                }

                target.Attributes["postalcode"] = endereco.Cep.GetOnlyNumbers();
                //target.Attributes["line2"] = enderecoIntegracao.Bairro;
                //target.Attributes["city"] = enderecoIntegracao.NomeCidade;
                target.Attributes["stateorprovince"] = enderecoIntegracao.UF;
                target.Attributes["country"] = enderecoIntegracao.Pais.Name;
            }
        }
    }
}