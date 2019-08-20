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
    public class PreCreate : IPlugin
    {
        public Organizacao Organizacao { get; set; }

        DynamicEntity EntidadeDoContexto = null;
        
        /// <summary>
        /// Verifica se o participante possui pontos para efetuar o resgate
        /// </summary>
        /// <param name="context">Conteudo do formulário de resgate</param>
        /// <exception cref="InvalidPluginExecutionException">Quantidade de pontos insuficiente para resgate, registre produtos para acumular pontos.</exception>
        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                if (context.InputParameters.Properties.Contains("Target") && context.InputParameters.Properties["Target"] is DynamicEntity) EntidadeDoContexto = (DynamicEntity)context.InputParameters.Properties["Target"];
                if (EntidadeDoContexto == null) return;

                this.Organizacao = new Organizacao(context.OrganizationName);

                Guid participanteId = Guid.Empty;
                int pontosResgate = 0;

                if (EntidadeDoContexto.Properties.Contains("new_participanteid")) participanteId = ((Lookup)EntidadeDoContexto.Properties["new_participanteid"]).Value;
                if (EntidadeDoContexto.Properties.Contains("new_quantidade_pontos_utilizados")) pontosResgate = ((CrmNumber)EntidadeDoContexto.Properties["new_quantidade_pontos_utilizados"]).Value;

                if (!DomainService.RepositoryCredito.PossuiPor(participanteId, pontosResgate, DateTime.Now))
                    throw new InvalidPluginExecutionException("Quantidade de pontos insuficiente para resgate, registre produtos para acumular pontos.");
            }
            catch (InvalidPluginExecutionException ex) { throw ex; }
            catch (Exception ex)
            {
                LogService.GravaLog(ex, TipoDeLog.PluginNew_resgate_premio_fidelidade);
                throw ex;
            }

        }
    }
}
