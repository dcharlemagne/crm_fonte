using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Sellout;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.ViewModels;
using Intelbras.CRM2013.Domain.Integracao;
using Intelbras.CRM2013.Domain.Servicos.GestaoSLA;

namespace Intelbras.CRM2013.Application.Plugin.itbc_feriado
{
    public class ManagerPostEventAssync : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            Entity entidade = new Entity();
            Feriado feriado = new Feriado(context.OrganizationName, context.IsExecutingOffline);

            switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
            {
                #region Create

                case Domain.Enum.Plugin.MessageName.Create:
                    entidade = (Entity)context.InputParameters["Target"];
                    feriado = entidade.Parse<Feriado>(context.OrganizationName, context.IsExecutingOffline);
                    List<Ocorrencia> lstOcorrenciasCreate = new Domain.Servicos.OcorrenciaService(context.OrganizationName, context.IsExecutingOffline).ListarOcorrenciasRecalculaSLA(feriado);

                    (new Domain.Servicos.RepositoryService()).Ocorrencia.Update(lstOcorrenciasCreate);

                    break;

                #endregion

                #region Update

                case Domain.Enum.Plugin.MessageName.Update:

                    var feriadoUpdade = ((Entity)context.PostEntityImages["imagem"]).Parse<Feriado>(context.OrganizationName, context.IsExecutingOffline, adminService);

                    List<Ocorrencia> lstOcorrenciasUpdate = new Domain.Servicos.OcorrenciaService(context.OrganizationName, context.IsExecutingOffline).ListarOcorrenciasRecalculaSLA(feriadoUpdade);

                    (new Domain.Servicos.RepositoryService()).Ocorrencia.Update(lstOcorrenciasUpdate);

                    break;

                    #endregion
            }
        }
    }
}
