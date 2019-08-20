using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;
using System;

namespace Intelbras.CRM2013.Application.Plugin.itbc_compdocanal
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            var compromissoCanalService = new CompromissosDoCanalService(context.OrganizationName, context.IsExecutingOffline, adminService);

            switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
            {
                case Domain.Enum.Plugin.MessageName.Create:

                    Entity entidade = (Entity)context.InputParameters["Target"];
                    CompromissosDoCanal compDoCanal = entidade.Parse<CompromissosDoCanal>(context.OrganizationName, context.IsExecutingOffline, adminService);

                    // Comentado devido problema com plugin do CRM2015 - não é possível alterar o OwnerId neste ponto - deixei comentado por enquanto
                    //var compromissosDoCanalService = new Domain.Servicos.CompromissosDoCanalService(context.OrganizationName, context.IsExecutingOffline, adminService);
                    //entidade = compromissosDoCanalService.AtribuiParaOProprietarioDoCanal(entidade, compDoCanal);
                    break;

                case Domain.Enum.Plugin.MessageName.Update:

                    var entityTargetUpdate = context.GetContextEntity();
                    var entityPreImagetUpdate = context.PreEntityImages["imagem"];

                    foreach (var item in entityTargetUpdate.Attributes)
                    {
                        entityPreImagetUpdate.Attributes[item.Key] = item.Value;
                    }

                    var compromissoCanalMergeUpdate = entityPreImagetUpdate.Parse<CompromissosDoCanal>(context.OrganizationName, context.IsExecutingOffline, adminService);

                    AtualizaDataValida(compromissoCanalMergeUpdate, compromissoCanalService, ref entityTargetUpdate);

                    break;
            }
        }

        private void AtualizaDataValida(CompromissosDoCanal compromissoCanal, CompromissosDoCanalService compromissosDoCanalService, ref Entity e)
        {
            if (e.Attributes.Contains("itbc_statuscompromissosid"))
            {              
                DateTime? validadeCompromissoCanal = compromissosDoCanalService.ObterValidade(compromissoCanal);

                if (validadeCompromissoCanal.HasValue)
                {
                    e.Attributes["itbc_validade"] = validadeCompromissoCanal.Value;
                }
            }
        }
    }
}