/*
 * Plugin deve ser registrado.
 * Filtering Attributes:   new_geratroca, new_ocorrenciaid, new_produtoid, new_qtd_solicitada, statuscode.
 * Stage:                  Synchronous - Regra de negócio exige para que a Intervenção Técnica seja criada junto com a Alteração.
 * Description:            SyncPostUpdate of new_diagnostico_ocorrencia in Parent Pipeline
 *
 * ***************************************************************
 * ***************************************************************
 * ***************************************************************
 * 
 * Registrar a Imagem 
 * Image Type:     Pre Image
 * Entity Alias:   preimage
 * Parameters:     description, regardingobjectid
 */


using System;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.SdkTypeProxy;
using Intelbras.Crm.CrossCutting.ConfigSistema;
using Intelbras.Crm.Domain.Services;
using Intelbras.Crm.Domain.Model;

namespace Intelbras.Crm.Application.Plugin.email
{
    public class PreManegerEvent : IPlugin
    {
        DynamicEntity entidade = null;

        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                if (context.InputParameters.Properties.Contains("Target"))
                {
                    if (context.InputParameters.Properties["Target"] is DynamicEntity)
                    {
                        entidade = (DynamicEntity)context.InputParameters.Properties["Target"];
                        ModeloEmailIsol(context);
                    }
                }
            }
            catch (Exception ex)
            {
                PluginHelper.TratarExcecao(ex, TipoDeLog.PluginEmail);
            }
        }

        private DynamicEntity GetPreImage(IPluginExecutionContext context)
        {
            DynamicEntity entity = null;
            if (context.PreEntityImages.Properties.Contains("preimage"))
            {
                if (context.PreEntityImages.Properties["preimage"] is DynamicEntity)
                {
                    entity = context.PreEntityImages.Properties["preimage"] as DynamicEntity;
                }

            }

            return entity;
        }

        protected string CriarLink(Guid id, string tipo, string organizationName)
        {
            string link = "";

            ConfiguracaoDeSistema config = new ConfiguracaoDeSistema() { OrganizationName = organizationName };
            config = config.ObterConfiguracaoDoCRM(tipo);

            if (config != null)
            {
                link = config.Valor;
                link += id;
            }

            return link;
        }

        protected void ModeloEmailIsol(IPluginExecutionContext context)
        {
            if (entidade.Properties.Contains("regardingobjectid")
                || entidade.Properties.Contains("description"))
            {

                Lookup regardingobjectid = null;
                string description = string.Empty;
                DynamicEntity preImage = GetPreImage(context);

                if (preImage != null)
                {
                    if (preImage.Properties.Contains("regardingobjectid"))
                    {
                        regardingobjectid = preImage.Properties["regardingobjectid"] as Lookup;
                    }

                    if (preImage.Properties.Contains("description"))
                    {
                        description = preImage.Properties["description"].ToString();
                    }
                }

                if (entidade.Properties.Contains("regardingobjectid"))
                {
                    regardingobjectid = entidade.Properties["regardingobjectid"] as Lookup;
                }

                if (entidade.Properties.Contains("description"))
                {
                    description = entidade.Properties["description"].ToString();
                }

                if (regardingobjectid != null)
                {

                    if (regardingobjectid.type == EntityName.incident.ToString())
                    {
                        if (description.Contains("{link_ocorrencia_isol}"))
                        {
                            string link = string.Format("<a href=\"{0}\">Clique Aqui</a>", CriarLink(regardingobjectid.Value, "LINK_ISOL_FECHAMENTO", context.OrganizationName));
                            description = description.Replace("{link_ocorrencia_isol}", link);
                        }

                        if (description.Contains("{link_ocorrencia_impressao}"))
                        {
                            string link = string.Format("<a href=\"{0}\">Clique Aqui</a>", CriarLink(regardingobjectid.Value, "LINK_ISOL_OS", context.OrganizationName));
                            description = description.Replace("{link_ocorrencia_impressao}", link);
                        }

                        if (description.Contains("{valor_linha_contrato}"))
                        {
                            var ocorrencia = new Ocorrencia() { Id = regardingobjectid.Value };
                            var linhaDeContrato = DomainService.RepositoryLinhaDoContrato.ObterPor(ocorrencia, "new_valor_pago");

                            string valorPago = (linhaDeContrato != null && linhaDeContrato.PrecoPago.HasValue) ? linhaDeContrato.PrecoPago.Value.ToString("F") : "0,00";

                            description = description.Replace("{valor_linha_contrato}", valorPago);
                        }

                        entidade = PluginHelper.AdicionarPropriedadeEmEntidadeDinamica(entidade, "description", description);
                    }
                }
            }
        }
    }
}
