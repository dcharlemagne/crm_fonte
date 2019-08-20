using Microsoft.Xrm.Sdk;
using SDKore.Configuration;
using System;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Application.Plugin.itbc_oportunidade
{
  
    public class ManagerPostEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
             Intelbras.CRM2013.Domain.Servicos.RepositoryService RepositoryService = new Intelbras.CRM2013.Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline, adminService);

            switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
            {
                case Domain.Enum.Plugin.MessageName.Create:

                    if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                    {
                        var target = (Entity)context.InputParameters["Target"];
                        var oportunidade = target.Parse<Domain.Model.Oportunidade>(context.OrganizationName, context.IsExecutingOffline, adminService);

                        try
                        {     
                            if(oportunidade.ClientePotencialOriginador != null)
                            {
                                #region Copia Arquivos do Sharepoint
                                    Domain.Model.ClientePotencial clientePotencial = RepositoryService.ClientePotencial.Retrieve(oportunidade.ClientePotencialOriginador.Id);
                                    List<Domain.Model.DocumentoSharePoint> lstDocs = RepositoryService.DocumentoSharePoint.ListarPorIdRegistro(oportunidade.ClientePotencialOriginador.Id);
                                    List<Domain.Model.Anotacao> lstAnotacoes = RepositoryService.Anotacao.ListarPor(oportunidade.ClientePotencialOriginador.Id);

                                    foreach (var doc in lstDocs)
                                    {
                                        Domain.Model.DocumentoSharePoint newDoc = doc;
                                        newDoc.ObjetoRelacionadoId = new SDKore.DomainModel.Lookup(oportunidade.ID.Value, "", SDKore.Crm.Util.Utility.GetEntityName(oportunidade));
                                        newDoc.Id = Guid.Empty;
                                        newDoc.ID = null;
                                        RepositoryService.DocumentoSharePoint.Create(newDoc);
                                    }

                                    foreach (var anotacao in lstAnotacoes)
                                    {
                                        Domain.Model.Anotacao newAnotacao = anotacao;
                                        newAnotacao.EntidadeRelacionada = new SDKore.DomainModel.Lookup(oportunidade.ID.Value, "", SDKore.Crm.Util.Utility.GetEntityName(oportunidade));
                                        newAnotacao.Id = Guid.Empty;
                                        newAnotacao.ID = null;
                                        RepositoryService.Anotacao.Create(newAnotacao);
                                    }
                                #endregion
                            }
                        }
                        catch (System.Exception e)
                        {
                            
                            throw;
                        }

                    }                    

                    break;
            }
        }
    }
}