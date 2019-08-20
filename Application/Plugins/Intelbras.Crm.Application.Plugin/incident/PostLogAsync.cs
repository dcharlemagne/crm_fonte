using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Services;
using Intelbras.Crm.Domain.Model.Enum;

namespace Intelbras.Crm.Application.Plugin.incident
{
    [Serializable]
    public class PostLogAsync : IPlugin
    {
        private IPluginExecutionContext Context = null;

        private DynamicEntity _preImage = null;
        private DynamicEntity PreImage
        {
            get
            {
                if (Context.PreEntityImages.Contains("Image") && (Context.PreEntityImages["Image"] is DynamicEntity))
                    _preImage = Context.PreEntityImages["Image"] as DynamicEntity;

                return _preImage;
            }
        }

        private DynamicEntity _postImage = null;
        private DynamicEntity PostImage
        {
            get
            {
                if (Context.PostEntityImages.Contains("Image") && (Context.PostEntityImages["Image"] is DynamicEntity))
                    _postImage = Context.PostEntityImages["Image"] as DynamicEntity;

                return _postImage;
            }
        }

        private DynamicEntity _inputImage = null;
        private DynamicEntity InputImage
        {
            get
            {
                if (this.Context.InputParameters.Properties.Contains("Target") && (this.Context.InputParameters.Properties["Target"] is DynamicEntity))
                    this._inputImage = this.Context.InputParameters.Properties["Target"] as DynamicEntity;

                return _inputImage;
            }
        }

        public void Execute(IPluginExecutionContext context)
        {
            DateTime inicioExecucao = DateTime.Now;
            try
            {
                this.Context = context;
                DomainService.Organizacao = new Organizacao(context.OrganizationName);


                DynamicEntity entity = this.GetDynamicEntity(context);

                PluginHelper.LogEmArquivo(context, "INICIO;", inicioExecucao.ToString(), "");

                // Quem alterou
                DomainService.RepositoryUsuario.Colunas = new string[] { "fullname", "new_grupo_callcenter" };
                Usuario usuario = DomainService.RepositoryUsuario.Retrieve(context.InitiatingUserId);


                LogOcorrencia log = new LogOcorrencia(DomainService.Organizacao);
                log.Nome = this.NomeDaAcao(context.MessageName);
                log.Alteracoes = this.MensageDeAlteracoes(entity);
                log.Categoria = this.GetCategoria(context);
                log.DataAlteracao = DateTime.Now;
                this.RegraEspecificaPorCategoria(ref log);


                // Usuário
                log.UsuarioId = usuario.Id;
                log.Usuario = usuario.NomeCompleto;
                log.GrupoProprietario = usuario.GrupoCallCenter;

                if (context.MessageName == MessageName.Create
                    || context.MessageName == MessageName.Update
                    || context.MessageName == MessageName.Assign)
                    log.OcorrenciaId = PluginHelper.GetEntityId(context);

                log.Salvar();
                PluginHelper.LogEmArquivo(context, "FIM;", inicioExecucao.ToString(), DateTime.Now.ToString());
            }
            catch (InvalidPluginExecutionException ex) { throw ex; }
            catch (Exception ex) 
            {
                LogService.GravaLog(ex, TipoDeLog.PluginIncident, "incident.PostUpdateLog");
                PluginHelper.LogEmArquivo(context, "ERRO;", inicioExecucao.ToString(), DateTime.Now.ToString());
            }
        }

        private string MensageDeAlteracoes(DynamicEntity entity)
        {
            if (entity == null) return string.Empty;

            var listaLabel = DomainService.RepositoryCadastro.ListarLabel(entity.Name);
            StringBuilder mensagem = new StringBuilder();
            string label = string.Empty;


            foreach (var item in entity.Properties)
                if (listaLabel.TryGetValue(item.Name, out label))
                    mensagem.AppendFormat("{0}: {1}\n", label, this.GetValueFromProperty(item));
                else
                    mensagem.AppendFormat("O campo {0} não foi encontrado\n", item.Name);

            return mensagem.ToString();
        }

        private string GetValueFromProperty(Microsoft.Crm.Sdk.Property inputProperty)
        {
            Type propType = inputProperty.GetType();
            string propValue = string.Empty;


            if (propType == typeof(Microsoft.Crm.Sdk.StringProperty))
                propValue = ((Microsoft.Crm.Sdk.StringProperty)inputProperty).Value;

            else if (propType == typeof(Microsoft.Crm.Sdk.CustomerProperty))
                propValue = ((Microsoft.Crm.Sdk.CustomerProperty)inputProperty).Value.Value.ToString();

            else if (propType == typeof(Microsoft.Crm.Sdk.CrmBooleanProperty))
                propValue = ((Microsoft.Crm.Sdk.CrmBooleanProperty)inputProperty).Value.Value.ToString();

            else if (propType == typeof(Microsoft.Crm.Sdk.CrmMoneyProperty))
                propValue = ((Microsoft.Crm.Sdk.CrmMoneyProperty)inputProperty).Value.Value.ToString();

            else if (propType == typeof(Microsoft.Crm.Sdk.CrmDateTimeProperty))
            {
                CrmDateTimeProperty dtProperty = (Microsoft.Crm.Sdk.CrmDateTimeProperty)inputProperty;
                if (dtProperty != null && dtProperty.Value != null)
                    propValue = dtProperty.Value.Value;
            }
            else if (propType == typeof(Microsoft.Crm.Sdk.CrmDecimalProperty))
                propValue = ((Microsoft.Crm.Sdk.CrmDecimalProperty)inputProperty).Value.Value.ToString();

            else if (propType == typeof(Microsoft.Crm.Sdk.CrmFloatProperty))
                propValue = ((Microsoft.Crm.Sdk.CrmFloatProperty)inputProperty).Value.Value.ToString();

            else if (propType == typeof(Microsoft.Crm.Sdk.CrmNumberProperty))
                propValue = ((Microsoft.Crm.Sdk.CrmNumberProperty)inputProperty).Value.Value.ToString();

            else if (propType == typeof(Microsoft.Crm.Sdk.LookupProperty))
            {
                if (inputProperty.Name.ToString() != "owningbusinessunit")
                    propValue = ((Microsoft.Crm.Sdk.LookupProperty)inputProperty).Value.Value.ToString();
            }
            else if (propType == typeof(Microsoft.Crm.Sdk.OwnerProperty))
                propValue = ((Microsoft.Crm.Sdk.OwnerProperty)inputProperty).Value.Value.ToString();

            else if (propType == typeof(Microsoft.Crm.Sdk.StatusProperty))
                propValue = ((Microsoft.Crm.Sdk.StatusProperty)inputProperty).Value.Value.ToString();

            else if (propType == typeof(Microsoft.Crm.Sdk.PicklistProperty))
                propValue = ((Microsoft.Crm.Sdk.PicklistProperty)inputProperty).Value.name;

            else if (propType == typeof(Microsoft.Crm.Sdk.KeyProperty))
                propValue = ((Microsoft.Crm.Sdk.KeyProperty)inputProperty).Value.Value.ToString();

            return propValue;
        }

        private string NomeDaAcao(string messageName)
        {
            switch (messageName)
            {
                case MessageName.Create: return "Criação";
                case MessageName.Update: return "Alteração";
                case MessageName.Assign: return "Proprietário";
                case MessageName.Delete: return "Exclusão";
                default: return string.Empty;
            }
        }

        private DynamicEntity GetDynamicEntity(IPluginExecutionContext context)
        {
            switch (context.MessageName)
            {
                case MessageName.Create:
                case MessageName.Update:
                    return this.InputImage;

                case MessageName.Assign:
                    return null;

                case MessageName.Delete:
                    return this.PreImage;
            }

            return null;
        }

        private CategoriaLogOcorrencia GetCategoria(IPluginExecutionContext context)
        {
            if (context.MessageName == MessageName.Assign)
                return Domain.Model.Enum.CategoriaLogOcorrencia.PorProprietario;

            if (context.MessageName == MessageName.Update && this.InputImage != null)
            {
                if (this.InputImage.Properties.Contains("statuscode"))
                    return CategoriaLogOcorrencia.PorStatus;

                if (this.InputImage.Properties.Contains("ownerid"))
                    return CategoriaLogOcorrencia.PorProprietario;
            }

            return CategoriaLogOcorrencia.Diversos;
        }

        private void RegraEspecificaPorCategoria(ref LogOcorrencia log)
        {
            switch (log.Categoria)
            {
                case CategoriaLogOcorrencia.PorProprietario:
                    SecurityPrincipal proprietarioNovo = ((SecurityPrincipal)this.Context.InputParameters.Properties["Assignee"]);
                    DomainService.RepositoryUsuario.Colunas = new string[] { "new_grupo_callcenter" };
                    Usuario usuario = DomainService.RepositoryUsuario.Retrieve(proprietarioNovo.PrincipalId);
                    log.GrupoDestino = usuario.GrupoCallCenter;
                    break;

                case CategoriaLogOcorrencia.PorStatus:
                    Status status = (Status)this.PostImage.Properties["statuscode"];
                    log.Status = status.name;
                    break;
            }
        }
    }
}
