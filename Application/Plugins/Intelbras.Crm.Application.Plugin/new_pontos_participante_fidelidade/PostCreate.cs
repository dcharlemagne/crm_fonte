using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using System.Diagnostics;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Infrastructure.Dal;
using Tridea.Framework.DomainModel;
using Intelbras.Crm.Domain.Model.Enum;
using System.Web.Services.Protocols;
using Intelbras.Crm.Domain.Repository;
using Intelbras.Crm.Domain.Services;

namespace Intelbras.Crm.Application.Plugin.new_pontos_participante_fidelidade
{
    public class PostCreate : IPlugin
    {
        public Organizacao Organizacao { get; set; }

        DynamicEntity EntidadeDoContexto = null;

        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                if (context.InputParameters.Properties.Contains("Target") && context.InputParameters.Properties["Target"] is DynamicEntity) EntidadeDoContexto = (DynamicEntity)context.InputParameters.Properties["Target"];
                if (EntidadeDoContexto == null) return;

                this.Organizacao = new Organizacao(context.OrganizationName);

                CreditoFidelidade creditoFidelidade = new CreditoFidelidade(Organizacao);

                creditoFidelidade.PontosParticipante = PluginHelper.GetEntityId(context);
                if (EntidadeDoContexto.Properties.Contains("new_name")) creditoFidelidade.Nome = EntidadeDoContexto.Properties["new_name"].ToString();
                if (EntidadeDoContexto.Properties.Contains("new_participanteid")) creditoFidelidade.GuidParticpante = ((Lookup)EntidadeDoContexto.Properties["new_participanteid"]).Value.ToString();
                if (EntidadeDoContexto.Properties.Contains("new_quantidade_pontos"))creditoFidelidade.ValorDisponivel = creditoFidelidade.Valor = ((CrmNumber)EntidadeDoContexto.Properties["new_quantidade_pontos"]).Value;
                if (EntidadeDoContexto.Properties.Contains("new_data_expiracao"))creditoFidelidade.DataVencimento = Convert.ToDateTime(((CrmDateTime)EntidadeDoContexto.Properties["new_data_expiracao"]).Value);

                DomainService.RepositoryCredito.Create(creditoFidelidade);
            }
            catch (Exception ex)
            {
                LogService.GravaLog(ex, TipoDeLog.PluginNew_pontos_participante_fidelidade);
            }

        }
    }
}
