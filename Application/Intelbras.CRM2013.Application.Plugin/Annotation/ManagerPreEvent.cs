using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Application.Plugin.Annotation
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            var annotationService = new Domain.Servicos.AnnotationService(context.OrganizationName, context.IsExecutingOffline, adminService);

            switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
            {
                case Domain.Enum.Plugin.MessageName.Create:

                    var entityTargetCreate = context.GetContextEntity();
                    var targetCreate = entityTargetCreate.Parse<Domain.Model.Anotacao>(context.OrganizationName, context.IsExecutingOffline, adminService);

                    if (targetCreate.Assunto == "") {
                        var guidProp = entityTargetCreate.GetAttributeValue<EntityReference>("ownerid");
                        var usuario = (new Domain.Servicos.RepositoryService()).Usuario.Retrieve(guidProp.Id);

                        entityTargetCreate.Attributes["subject"] = "Anotação criada em " + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + " por " + usuario.NomeCompleto;
                    }
                    break;

            }
        }
    }
}