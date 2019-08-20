using System;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Application.Workflow.Konviva
{
    public class UpdateKonviva : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
                IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
                IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);


                Guid? canalId = null;

                if (workflowContext.InputParameters.Contains("Target"))
                {
                    var e = (Entity)workflowContext.InputParameters["Target"];

                    if (e.LogicalName == SDKore.Crm.Util.Utility.GetEntityName<Conta>())
                        canalId = e.Id;

                    if (e.LogicalName == SDKore.Crm.Util.Utility.GetEntityName<CategoriasCanal>())
                    {
                        var categoriaModel = e.Parse<CategoriasCanal>(workflowContext.OrganizationName, workflowContext.IsExecutingOffline);
                       
                        canalId = categoriaModel.Canal.Id;
                    }

                    new DeParaDeUnidadeDoKonvivaService(workflowContext.OrganizationName, workflowContext.IsExecutingOffline).AtualizarUnidadeKonvivaDosContatosDoCanal(canalId);
                }
            }
            catch (Exception e)
            {
                throw new InvalidWorkflowException(e.Message + " :: " + e.StackTrace, e);
            }
        }
    }
}
