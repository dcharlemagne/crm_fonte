using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services.Protocols;
using Microsoft.Crm.Sdk;
using Tridea.Framework.DomainModel;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Infrastructure.Dal;
using Intelbras.Crm.Domain.Services;
using Intelbras.Crm.Domain.Model.Enum;
using Intelbras.Helper.Log;

namespace Intelbras.Crm.Application.Plugin.incident
{
    public class PostUpdateAsync : IPlugin
    {
        Organizacao Organizacao = null;
        DynamicEntity EntidadeDoContexto = null;

        public void Execute(IPluginExecutionContext context)
        {
            DateTime inicioExecucao = DateTime.Now;
            try
            {
                if ((context.InputParameters.Properties.Contains("Target") && context.InputParameters.Properties["Target"] is DynamicEntity)) this.EntidadeDoContexto = context.InputParameters.Properties["Target"] as DynamicEntity;
                if (this.EntidadeDoContexto == null) return;

                PluginHelper.LogEmArquivo(context, "INICIO;", inicioExecucao.ToString(), "");

                #region Recupera a Ocorrencia atualizada

                this.Organizacao = new Organizacao(context.OrganizationName);
                var id = PluginHelper.GetEntityId(context);
                //Retirado o Retrieve pois a função carrega vários sub-dados, como informações de cliente, nota fiscal, produto, etc, etc, etc
                var ocorrencia = DomainService.RepositoryOcorrencia.RetrieveBasico(id, "new_guid_endereco", "statuscode", "casetypecode", "new_data_hora_conclusao");

                #endregion


                this.AtualizaVigencia(ocorrencia);

                FacadeOcorrencia facade = new FacadeOcorrencia(context);
                facade.PosAlteracao();
                PluginHelper.LogEmArquivo(context, "FIM;", inicioExecucao.ToString(), DateTime.Now.ToString());
            }
            catch (Exception ex) 
            {
                PluginHelper.TratarExcecao(ex, TipoDeLog.PluginIncident);
                PluginHelper.LogEmArquivo(context, "ERRO;", inicioExecucao.ToString(), DateTime.Now.ToString());
            }
        }

        private void AtualizaVigencia(Ocorrencia ocorrencia)
        {
            try
            {
                if (ocorrencia.StatusDaOcorrencia != StatusDaOcorrencia.Fechada)
                    return;

                OcorrenciaService service = new OcorrenciaService(ocorrencia);
                service.AtualizarVigenciaContrato();
            }
            catch (Exception ex)
            {
                LogHelper.Process(ex, ClassificacaoLog.PluginIncident);
            }
        }
    }
}