using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Model.Treinamentos;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Model.Ocorrencias;
using System.Diagnostics;
using Tridea.Framework.DomainModel;

namespace Intelbras.Crm.Application.Plugin.new_lancamento_avulso
{
    public class PreDelete : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            if (!context.PreEntityImages.Contains("Target"))
            {
                string erro = String.Format("Houve um problema ao executar o plugin Extrato (PreDelete): Mensagem: Imagem para o step PreDelete não configurado!");
                EventLog.WriteEntry("CRM Lancamento Avulso", erro);
                throw new Exception(erro);
            }

            DynamicEntity entity = (DynamicEntity)context.PreEntityImages["Target"];
            Lookup lkpExtrato = (Lookup)entity.Properties["new_extratoid"];

            if (lkpExtrato.Value != Guid.Empty)
            {
                try
                {
                    var todosAsOcorrencias = RepositoryFactory.GetRepository<ITodasAsOcorrencias>();
                    todosAsOcorrencias.Organizacao = new Organizacao(context.OrganizationName);
                    todosAsOcorrencias.AtualizaValoresDosLancamentosAvulsosNo(lkpExtrato.Value);
                }
                catch (Exception ex)
                {
                    string erro = String.Format("Houve um problema ao executar o plugin Extrato (PreDelete): Mensagem: {0} -- StackTrace: {1} \n--{2}", ex.Message, ex.StackTrace, ex.InnerException);
                    EventLog.WriteEntry("CRM Lancamento Avulso", erro);
                    throw new Exception(erro);
                }
            }
        }
    }
}
