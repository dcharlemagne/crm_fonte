using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using System.Diagnostics;
using Intelbras.Crm.Domain.Model.Contratos;
using Intelbras.Crm.Domain.Model;

namespace Intelbras.Crm.Application.Plugin.contractline
{
    public class PostCreate : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            throw new Exception("It works....");

            try
            {
                DynamicEntity entity = null;
                var isDynamicEntity = (context.InputParameters.Properties.Contains("Target") && context.InputParameters.Properties["Target"] is DynamicEntity);

                if (isDynamicEntity)
                {
                    entity = context.InputParameters.Properties["Target"] as DynamicEntity;
                    this.CriaLinhaDeContrato(entity, new Organizacao(context.OrganizationName));
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", String.Format("Houve um problema ao executar o plugin 'ContractLine.PostCreate': Mensagem: {0} -- StackTrace: {1}", ex.Message, ex.StackTrace));
            }
        }

        #region Privados

        private void CriaLinhaDeContrato(DynamicEntity entity, Organizacao organizacao)
        {
            var contratoId = entity.Properties["new_contractid"] ?? null;
            if (contratoId != null)
            {
                var contrato = new Contrato(organizacao);

                contrato.Id = new Guid(contratoId.ToString());

                var inicioVigenciaContrato = entity.Properties["activeon"] ?? null;
                if (inicioVigenciaContrato != null)
                    contrato.InicioVigencia = Convert.ToDateTime(inicioVigenciaContrato);
                else
                    contrato.InicioVigencia = DateTime.Now;

                contrato.CriarLinhaDeContrato(contrato);
            }
            else
                throw new ArgumentException("O ID do contrato não está preenchido.");
        }

        protected void EscreverNoEventViewer(string mensagem, string erro)
        {
            EventLog.WriteEntry("Application", String.Format("MSG: {0} -- {1}", mensagem, erro));
        }

        #endregion
    }
}
