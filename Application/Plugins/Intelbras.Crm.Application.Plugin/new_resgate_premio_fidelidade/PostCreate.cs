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

namespace Intelbras.Crm.Application.Plugin.new_resgate_premio_fidelidade
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

                Guid resgateId = Guid.Empty;
                Guid participanteId = Guid.Empty;
                int valorResgate = 0;
                int contadorDebito = 0;
                bool parar = false;

                resgateId = PluginHelper.GetEntityId(context);
                if (EntidadeDoContexto.Properties.Contains("new_participanteid")) participanteId = ((Lookup)EntidadeDoContexto.Properties["new_participanteid"]).Value;
                if (EntidadeDoContexto.Properties.Contains("new_quantidade_pontos_utilizados")) valorResgate = ((CrmNumber)EntidadeDoContexto.Properties["new_quantidade_pontos_utilizados"]).Value;

                List<CreditoFidelidade> listaCreditos = DomainService.RepositoryCredito.ListarDisponiveisPor(participanteId, DateTime.Now);

                foreach (CreditoFidelidade item in listaCreditos)
                {
                    DebitoFidelidade debito = new DebitoFidelidade();
                    debito.Nome = "Débito " + contadorDebito.ToString();
                    debito.GuidParticpante = participanteId.ToString();
                    debito.Credito = item.Id;
                    debito.Resgate = resgateId;
                    debito.Situacao = 1;
                    contadorDebito += 1;

                    if (item.ValorDisponivel < valorResgate)
                    {
                        debito.Valor = item.ValorDisponivel;
                        valorResgate -= debito.Valor;
                        item.ValorDisponivel = 0;
                    }
                    else
                    {
                        debito.Valor = valorResgate;
                        item.ValorDisponivel -= valorResgate;
                        valorResgate = 0;
                        parar = true;
                    }

                    DomainService.RepositoryDebito.Create(debito);
                    DomainService.RepositoryCredito.Update(item);

                    if (parar) break;
                }

            }
            catch (Exception ex)
            {
                LogService.GravaLog(ex, TipoDeLog.PluginNew_resgate_premio_fidelidade);
            }

        }

    }
}
