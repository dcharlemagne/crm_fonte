using Microsoft.Xrm.Sdk;
using System;

namespace Intelbras.CRM2013.Application.Plugin.itbc_postagem
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            Entity entidade = new Entity();
            Domain.Model.Postagem post = new Domain.Model.Postagem(context.OrganizationName, context.IsExecutingOffline);
            var repositoryService = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline);

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    case Domain.Enum.Plugin.MessageName.Create:
                        entidade = (Entity)context.InputParameters["Target"];
                        post = entidade.Parse<Domain.Model.Postagem>(context.OrganizationName, context.IsExecutingOffline);

                        try
                        {
                            //adiciona data/hora no texto da postagem
                            if (!post.Texto.StartsWith("<?xml"))
                                entidade.Attributes["text"] = string.Format("Em: {0}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")) + Environment.NewLine + post.Texto;

                            //postagem do Lead
                            Guid guidLead = new Guid(post.ReferenteA.Id.ToString());
                            var cliente = repositoryService.ClientePotencial.Retrieve(guidLead);
                            if (cliente != null)
                            {
                                repositoryService.ClientePotencial.Update(cliente);
                            }
                            else
                            {
                                var oportunidade = repositoryService.Oportunidade.Retrieve(guidLead);
                                if (oportunidade != null)
                                {
                                    repositoryService.Oportunidade.Update(oportunidade);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                        }

                        break;

                    case Domain.Enum.Plugin.MessageName.Update:

                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            entidade = (Entity)context.PostEntityImages["imagem"];
                            post = entidade.Parse<Domain.Model.Postagem>(context.OrganizationName, context.IsExecutingOffline);

                            try
                            {
                                Guid guidLead = new Guid(post.ReferenteA.Id.ToString());
                                var cliente = repositoryService.ClientePotencial.Retrieve(guidLead);
                                if (cliente != null)
                                {
                                    repositoryService.ClientePotencial.Update(cliente);
                                }
                                else
                                {
                                    var oportunidade = repositoryService.Oportunidade.Retrieve(guidLead);
                                    if (oportunidade != null)
                                    {
                                        repositoryService.Oportunidade.Update(oportunidade);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                            }
                        }

                        break;
                }
            }
        }
    }
}